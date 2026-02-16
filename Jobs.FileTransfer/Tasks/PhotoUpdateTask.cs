using Brupper.Jobs.FileTransfer.Azure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer.Tasks
{
    /// <summary>
    /// Manages photo update tasks.
    /// This is a child class of the <see cref="Brupper.Jobs.Tasks.AbstractPhotoTask"/> class
    /// </summary>
    public class PhotoUpdateTask : AbstractPhotoTask, IBackgroundTask<string>
    {
        /// <summary> The task id of the photo update task. </summary>
        public const string TaskIdTemplate = "PhotoUpdateTask:{0}";

        private IUploader uploader;

        #region Constructors

        public PhotoUpdateTask(
            IFileUploader fileUploader,
            ITokenService tokenService,
            IFileProxyServiceFactory fileProxyServiceFactory,
            IBackgroundTaskLogRepository uploadTaskLogLocalService,
            IBackgroundTaskRepository uploadTaskLocalService,
            IUploader uploader,
            ILogger<PhotoUpdateTask> logger)
            : base(fileUploader, tokenService, fileProxyServiceFactory, uploadTaskLogLocalService, uploadTaskLocalService, logger)
        {
            this.uploader = uploader;
        }

        #endregion

        #region AbstractTask implementation

        protected override async Task InternalExecuteAsync()
        {
            //Logger.Current?.TrackTrace(LogTag.TaskChangeEvent, "Photo update started", new Metadata(LoggingLabels.PhotoIdLabel, Photo.Uuid));

            await CheckIfExistOnServer();

            if (Photo == null)
            {
                SetStatus(TaskStatus.Error, string.Format($"Photo object was empty in PhotoUpdateTask"));
            }

        }

        public override void ProcessRelatedTasks(IEnumerable<IBackgroundTask> tasks)
        {
            var tmpTaskLIst = tasks?.ToList() ?? new List<IBackgroundTask>();
            if (tmpTaskLIst.Any(t => t is PhotoDeleteTask deleteTask && deleteTask.Photo == Photo))
            {
                // upload task for the same uuid photo cannot run after a delete task,
                // check for existing delete task for the same photo and abort itself 
                Abort();
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
