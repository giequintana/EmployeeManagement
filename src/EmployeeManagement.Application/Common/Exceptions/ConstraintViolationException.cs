namespace EmployeeManagement.Application.Common.Exceptions;

public enum ConstraintViolationKind
{
    Unique,
    ForeignKey,
    Check
}

public sealed class ConstraintViolationException(ConstraintViolationKind kind, Exception innerException)
    : Exception($"A database {kind} constraint was violated.", innerException)
{
    public ConstraintViolationKind Kind { get; } = kind;
}