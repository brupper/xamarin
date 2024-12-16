using Brupper.Jobs.FileTransfer.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer.Tasks
{
    /// <summary>
    /// Manages photo upload tasks.
    /// This is a child class of the <see cref="AbstractPhotoTask"/> class
    /// </summary>
    public class PhotoUploadTask : AbstractPhotoTask, IBackgroundTask<string>
    {
        /// <summary> The task id of the photo upload task. </summary>
        public const string TaskIdTemplate = "PhotoUploadTask:{0}";

        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="PhotoUploadTask"/> class. </summary>
        public PhotoUploadTask(
            IFileUploader fileUploader,
            ITokenService tokenService,
            IFileProxyServiceFactory fileProxyServiceFactory,
            IBackgroundTaskLogRepository uploadTaskLogLocalService,
            IBackgroundTaskRepository uploadTaskLocalService,
            ILogger<PhotoUploadTask> logger)
            : base(fileUploader, tokenService, fileProxyServiceFactory, uploadTaskLogLocalService, uploadTaskLocalService, logger)
        { }

        #endregion

        #region AbstractTask implementation

        protected override async Task InternalExecuteAsync()
        {
            var sasToken = await tokenService.FetchSasTokenAsync();
            var blobService = fileProxyServiceFactory.CreateFromSasToken(sasToken);

            try
            {
                await blobService.UpdateBlobContentAsync(Photo, new System.IO.MemoryStream(System.IO.File.ReadAllBytes(Photo)), default);
                SetStatus(TaskStatus.Completed, string.Format($"Photo {Photo} created with id: {Photo}"));
            }
            catch (Exception e)
            {
                SetStatus(TaskStatus.Error, string.Format($"Error when creating photo {Photo} on server, Error {e}"));
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

        protected override Task OnProgressAsync(double progress)
        {
            // Photo.UploadProgress = progress;
            // messenger.Publish(new UploadProgressMessage<Photo>(Photo, progress, this));

            logger.LogTrace(new EventId(0, "porressreport"), $"[{DateTime.Now.TimeOfDay}] The Photo {Photo} uploading progress: {progress}");

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private async Task FinishFailedUpload(UploadFinishedResponse result, string reason = "Unknown error")
        {
            logger.LogError(reason, result.Content);

            SetStatus(TaskStatus.Error, string.Format(reason + $" when creating photo {Photo}"));

            var remotePhoto = await CheckForRemotePhotoAsync();
            if (remotePhoto != null)
            {
                SetStatus(TaskStatus.Completed, string.Format(reason + $", photo/file {Photo} updated with id: {Photo}"));
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
