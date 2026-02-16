using Brupper.Jobs.FileTransfer.Azure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer.Tasks
{
    /// <summary>
    /// Manages photo delete tasks.
    /// This is a child class of the <see cref="Brupper.Jobs.Tasks.AbstractPhotoTask"/> class
    /// </summary>
    public class PhotoDeleteTask : AbstractPhotoTask, IBackgroundTask<string>
    {
        /// <summary> The task id of the photo delete task. </summary>
        public const string TaskIdTemplate = "PhotoDeleteTask:{0}";

        #region Constructors

        public PhotoDeleteTask(
            IFileUploader fileUploader,
            ITokenService tokenService,
            IFileProxyServiceFactory fileProxyServiceFactory,
            IBackgroundTaskLogRepository uploadTaskLogLocalService,
            IBackgroundTaskRepository uploadTaskLocalService,
            ILogger<PhotoDeleteTask> logger)
            : base(fileUploader, tokenService, fileProxyServiceFactory, uploadTaskLogLocalService, uploadTaskLocalService, logger)
        { }

        #endregion

        #region AbstractTask implementation

        protected override async Task InternalExecuteAsync()
        {
            await CheckIfExistOnServer();



        }

        public override void ProcessRelatedTasks(IEnumerable<IBackgroundTask> tasks)
        {
            if (tasks != null && !tasks.Empty())
            {
                var uploadTasksForSamePhoto = tasks.Where(t => t is PhotoUploadTask uploadTask && uploadTask.Photo == Photo);
                var updateTasksForSamePhoto = tasks.Where(t => t is PhotoUpdateTask updateTask && updateTask.Photo == Photo);

                foreach (var task in uploadTasksForSamePhoto.Union(updateTasksForSamePhoto))
                {
                    task.Abort();
                }

                // Scenario: when upload (request) is cancelled locally, server can receive the request and process it, so creates a remote entity for photo.
                // By cancelling upload we do requests like DDoS. In this case there is a remote entity, we do not know about that and we just abort the
                // delete task locally and do not call remote delete.
                // We must not abort PhotoDeleteTask, because it checks the remote id and deletes remote photo that cancelled at uploading (...but server got the request).
                // This caused duplicated ranks bug: if (uploadTasksForSamePhoto.Any() && uploadTasksForSamePhoto.All(x => x.IsAborted)) { Abort(); }
            }
        }

        #endregion

        protected override string GetTaskId(string photo)
        {
            return GetId(photo);
        }

        /// <summary> Returns the id of the task. </summary>
        /// <param name="photo">The photo.</param>
        /// <returns>The id of the task.</returns>
        public static string GetId(string photo)
        {
            return string.Format(CultureInfo.InvariantCulture, TaskIdTemplate, photo);
        }
    }
}
