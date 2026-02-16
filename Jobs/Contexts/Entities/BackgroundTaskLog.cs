using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Brupper.Jobs;

/// <summary> Object for uploading task logs to the server. </summary>
public class BackgroundTaskLog
{
    #region Public Properties

    /// <summary> The task id. </summary>
    public string TaskId { get; set; } = default!;

    /// <summary> The string version of task status. </summary>
    public string TaskStatus { get; set; } = default!;

    /// <summary> The comment. </summary>
    public string Comment { get; set; } = default!;

    /// <summary> The string version of the date when the task status is changed. </summary>
    public string DateString { get; set; } = default!;

    #endregion

    #region Ignore

    /// <summary> The date when the task status is changed. </summary>
    [NotMapped, JsonIgnore]
    public DateTime Date
    {
        get => DateTimeHelper.FromRoundTrip(DateString);
        set => DateString = DateTimeHelper.ToRoundTrip(value);
    }

    /// <summary> The task status. </summary>
    [NotMapped, JsonIgnore]
    public TaskStatus Status
    {
        get => (TaskStatus)Enum.Parse(typeof(TaskStatus), TaskStatus);
        set => TaskStatus = Enum.GetName(typeof(TaskStatus), value);
    }

    #endregion
}
