namespace EmployeeManagement.Application.Mappings;

internal static class TextNormalization
{
    public static string? Optional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}