using CommunityToolkit.Diagnostics;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brupper.Jobs;

/// <summary> Object for uploading tasks to the server. </summary>
public class BackgroundTask
{
    #region Public Properties

    /// <summary> The task id. </summary>
    [Key]
    public string TaskId { get; set; } = default!;

    /// <summary> The rank. </summary>
    public int Rank { get; set; }

    /// <summary> The string version of task type. </summary>
    public string TaskTypeString { get; set; } = default!;

    /// <summary> The string version of task status.</summary>
    public string TaskStatus { get; set; } = default!;

    /// <summary> Global unique id of referenced item.</summary>
    public string ItemId { get; set; } = default!;

    /// <summary> The string version of the task's last modification date. </summary>
    public string ModifiedAtString { get; set; } = default!;

    /// <summary> The string version of the task's creation date. </summary>
    public string CreatedAtString { get; set; } = default!;

    #endregion

    #region Constructors

    /// <summary> Empty constructor for using <see cref="Brupper.Jobs.BackgroundTask"/> class as generic. </summary>
    public BackgroundTask() { }

    /// <summary> Initializes a new instance of the <see cref="Brupper.Jobs.BackgroundTask"/> class. </summary>
    public BackgroundTask(IBackgroundTask task)
    {
        Guard.IsNotNull(task);

        TaskType = task.GetType();
        TaskId = task.Id;
        Status = task.Status;
        ModifiedAt = task.LastModificationDate;
        CreatedAt = task.CreationDate;
        Rank = task.Rank;

        if (task.EntityId != default)
        {
            ItemId = task.EntityId;
        }
    }

    #endregion

    #region Ignore / Not Mapped properties

    /// <summary> The task type. </summary>
    //[Ignore]
    [NotMapped]
    public Type TaskType
    {
        get => Type.GetType(TaskTypeString);
        set => TaskTypeString = value.ToString();
    }

    /// <summary> The task status. </summary>
    //[Ignore]
    [NotMapped]
    public TaskStatus Status
    {
        get => (TaskStatus)Enum.Parse(typeof(TaskStatus), TaskStatus);
        set => TaskStatus = Enum.GetName(typeof(TaskStatus), value);
    }

    /// <summary> The task's last modification date. </summary>
    //[Ignore]
    [NotMapped]
    public DateTime ModifiedAt
    {
        get => DateTime.Parse(ModifiedAtString);
        set => ModifiedAtString = DateTimeHelper.ToRoundTrip(value);
    }

    /// <summary> The task's creation date.</summary>
    //[Ignore]
    [NotMapped]
    public DateTime CreatedAt
    {
        get => DateTime.Parse(CreatedAtString);
        set => CreatedAtString = DateTimeHelper.ToRoundTrip(value);
    }

    #endregion
}
