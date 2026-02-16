using Brupper.Jobs.FileTransfer.Azure;
using Brupper.Jobs.Tasks;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer.Tasks;

/// <summary> This is a child class of the <see cref="AbstractTransferTask"/> class </summary>
public abstract class AbstractPhotoTask : AbstractTransferTask
{
    public string Photo { get; protected set; }

    #region Fields

    protected readonly IFileUploader fileUploader;
    protected readonly ITokenService tokenService;
    protected readonly IFileProxyServiceFactory fileProxyServiceFactory;

    #endregion

    #region Constructors

    protected AbstractPhotoTask(
        IFileUploader fileUploader,
        ITokenService tokenService,
        IFileProxyServiceFactory fileProxyServiceFactory,
        IBackgroundTaskLogRepository taskLogRepository,
        IBackgroundTaskRepository taskRepository,
        ILogger logger)
        : base(taskLogRepository, taskRepository, logger)
    {
        Guard.IsNotNull(fileUploader, nameof(fileUploader));
        Guard.IsNotNull(tokenService, nameof(tokenService));
        Guard.IsNotNull(fileProxyServiceFactory, nameof(fileProxyServiceFactory));

        this.fileUploader = fileUploader;
        this.tokenService = tokenService;
        this.fileProxyServiceFactory = fileProxyServiceFactory;
    }

    #endregion

    #region AbstractTask implementation

    public override async Task DeserializeAsync(string taskId)
    {
        var photoUuid = GetMetadataIdFromTaskId(taskId, TransferErrorCode.InvalidPhotoTaskId);
        var photo = photoUuid.ToString(); //await photoRepository.GetLocalPhotoAsync(photoUuid);

        if (photo == null)// TODO: || photo.Deleted)
        {
            Error = TransferErrorCode.PhotoDeserializationError;
            throw new Exception(string.Format($"Cannot find photo {photoUuid}"));
        }

        await InitAsync(photo);
    }

    protected override Task OnProgressAsync(double progress)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region ITask<Photo> implementation

    public async Task InitAsync(string photo)
    {
        Guard.IsNotNull(photo, nameof(photo));

        this.Photo = photo;
        Id = GetTaskId(photo);
        // Audit = await repository.GetLocalAsync(photo.Visit.Uuid);
    }

    #endregion

    protected abstract string GetTaskId(string photo);


    protected async Task<string> CheckForRemotePhotoAsync()
    {
        return null;
        /*
        var remotePhoto = await photoRepository.GetRemotePhotoAsync(Photo.Uuid, Audit, null, CancellationToken.None);
        if (remotePhoto == null)
        {
            Logger.Current?.TrackTrace(LogTag.Error, "Remote photo does not exist", new Metadata(LoggingLabels.PhotoIdLabel, Photo.Uuid));
        }
        else
        {
            Photo = remotePhoto;
            await photoRepository.UpdateFromRemote(Photo);
        }

        return remotePhoto;
        // */
    }

}
