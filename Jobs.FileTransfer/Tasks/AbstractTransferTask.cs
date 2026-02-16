using Brupper.Jobs.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer.Tasks;

/// <summary>
/// <see cref="Brupper.Jobs.IBackgroundTask"/> implementation.
/// This is the base class of the background tasks.
/// </summary>
public abstract class AbstractTransferTask(
    IBackgroundTaskLogRepository taskLogRepository,
    IBackgroundTaskRepository taskRepository,
    ILogger logger)
    : AbstractTask(taskLogRepository, taskRepository, logger)
{
    #region Protected Methods

    [Obsolete("ez mar inkabb filetransfer cucc")]
    protected async Task CheckIfExistOnServer()
    {
        await Task.CompletedTask;

        /*
        if (Audit != null && Audit.ExistsOnServer)
        {
            return;
        }

        if (Audit != null)
        {
            var localAudit = await repository.GetLocalAsync(Audit.Uuid);
            if (localAudit != null)
            {
                Audit = localAudit;
            }
        }

        if (Audit != null && !Audit.ExistsOnServer)
        {
            await TryToSetFromRemote();
        }

        if (this.Audit == null || !this.Audit.ExistsOnServer)
        {
            if (Audit != null)
            {
                var errorMessage = $"Visit {this.Audit.Uuid} was not uploaded yet";
                SetStatus(TaskStatus.Error, errorMessage);

                throw new WasNotUploadedToServerException(errorMessage);
            }
            else
            {
                var errorMessage = $"The audit was empty from the repository";
                SetStatus(TaskStatus.Error, errorMessage);

                throw new IsNullDuringUploadException(errorMessage);
            }
        }
        // */
    }

    [Obsolete("ez mar inkabb filetransfer cucc")]
    private async Task TryToSetFromRemote()
    {
        await Task.CompletedTask;
        /*
        try
        {
            var remoteAudit = await repository.GetRemoteAsync(Audit, new CancellationToken());
            if (remoteAudit != null)
            {
                await Repository.UpdateAsync(remoteAudit);
                Audit = await repository.GetLocalAsync(remoteAudit.Uuid);
            }
        }
        catch (Exception e)
        {
            Logger.Current?.TrackEvent("Error happened during item set by remote");
            logger.LogError($"{e}");
        }
        */
    }

    #endregion
}
