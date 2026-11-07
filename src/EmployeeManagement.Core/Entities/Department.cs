namespace EmployeeManagement.Core.Entities;

public class Department : BaseEntity
{
    public string DepartmentName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}