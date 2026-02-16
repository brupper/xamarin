using System;
using System.Diagnostics.CodeAnalysis;

namespace Brupper.Jobs;

/// <summary> Represents all the task error supported </summary>
public enum TaskError
{
    None,
    Unknown = 1,
    Cancelled = 2,

    InvalidTaskId = 100,
    DeserializationError = 101,
    RemoteCreation = 102,
    CreationForbidden = 103,
}

public partial class TaskErrorCode : IEquatable<TaskErrorCode>
{
    private readonly string _method;
    private int _hashcode;

    public static TaskErrorCode Unknown { get; } = new(nameof(TaskError.Unknown));
    public static TaskErrorCode Cancelled { get; } = new(nameof(TaskError.Cancelled));
    public static TaskErrorCode InvalidTaskId { get; } = new(nameof(TaskError.InvalidTaskId));
    public static TaskErrorCode DeserializationError { get; } = new(nameof(TaskError.DeserializationError));
    public static TaskErrorCode RemoteCreation { get; } = new(nameof(TaskError.RemoteCreation));
    public static TaskErrorCode CreationForbidden { get; } = new(nameof(TaskError.CreationForbidden));
    // public static TaskErrorCode  { get; } = new(nameof(TaskError.));

    public string Method => _method;

    public TaskErrorCode(string method)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(method);

        _method = method;
    }

    public bool Equals([NotNullWhen(true)] TaskErrorCode? other) => other is not null && string.Equals(_method, other._method, StringComparison.OrdinalIgnoreCase);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is TaskErrorCode method && Equals(method);

    public override int GetHashCode()
    {
        if (_hashcode == 0)
        {
            _hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode(_method);
        }

        return _hashcode;
    }

    public override string ToString() => _method;

    public static bool operator ==(TaskErrorCode? left, TaskErrorCode? right) => left is null || right is null ? ReferenceEquals(left, right) : left.Equals(right);

    public static bool operator !=(TaskErrorCode? left, TaskErrorCode? right) => !(left == right);

    /// <summary>Parses the provided <paramref name="method"/> into an <see cref="TaskErrorCode"/> instance.</summary>
    /// <param name="method">The method to parse.</param>
    /// <returns>An <see cref="TaskErrorCode"/> instance for the provided <paramref name="method"/>.</returns>
    public static TaskErrorCode Parse(ReadOnlySpan<char> method) => new(method.ToString());
}