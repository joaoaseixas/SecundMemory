namespace SecundMemory.Domain.Common;

/// <summary>
/// Outcome of an operation that can fail for expected business reasons.
/// Reserve exceptions for the unexpected; use Result for "this is a normal,
/// anticipated way for this operation to not succeed."
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && error != string.Empty)
            throw new InvalidOperationException("A successful result cannot have an error message.");
        if (!isSuccess && error == string.Empty)
            throw new InvalidOperationException("A failed result must have an error message.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    protected internal Result(T? value, bool isSuccess, string error) : base(isSuccess, error) => _value = value;

    public static implicit operator Result<T>(T value) => Success(value);
}
