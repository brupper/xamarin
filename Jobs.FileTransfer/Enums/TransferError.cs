namespace Brupper.Jobs.FileTransfer;

/// <summary> Represents all the task error supported </summary>
public enum TransferError
{
    None,
    Unknown = 1,
    Cancelled = 2,

    InvalidPhotoTaskId = 200,
    PhotoDeserializationError = 210,
    RemotePhotoCreation = 220,
    PhotoCreationBadRequest = 221,
    PhotoCreationInternalServerError = 222,
    PhotoCreationCreationForbidden = 223,

    RemotePhotoDelete = 230,
    PhotoDeleteBadRequest = 231,
    PhotoDeleteInternalServerError = 232,
    PhotoDeleteForbidden = 233,
}

public class TransferErrorCode(string method) : TaskErrorCode(method)
{
    public static TaskErrorCode InvalidPhotoTaskId { get; } = new(nameof(TransferError.InvalidPhotoTaskId));
    public static TaskErrorCode PhotoDeserializationError { get; } = new(nameof(TransferError.PhotoDeserializationError));
    public static TaskErrorCode RemotePhotoCreation { get; } = new(nameof(TransferError.RemotePhotoCreation));
    public static TaskErrorCode PhotoCreationBadRequest { get; } = new(nameof(TransferError.PhotoCreationBadRequest));
    public static TaskErrorCode PhotoCreationInternalServerError { get; } = new(nameof(TransferError.PhotoCreationInternalServerError));
    public static TaskErrorCode PhotoCreationCreationForbidden { get; } = new(nameof(TransferError.PhotoCreationCreationForbidden));
    public static TaskErrorCode RemotePhotoDelete { get; } = new(nameof(TransferError.RemotePhotoDelete));
    public static TaskErrorCode PhotoDeleteBadRequest { get; } = new(nameof(TransferError.PhotoDeleteBadRequest));
    public static TaskErrorCode PhotoDeleteInternalServerError { get; } = new(nameof(TransferError.PhotoDeleteInternalServerError));
    public static TaskErrorCode PhotoDeleteForbidden { get; } = new(nameof(TransferError.PhotoDeleteForbidden));
}