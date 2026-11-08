using EmployeeManagement.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DepartmentName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.HasIndex(d => d.DepartmentName)
            .IsUnique();

        builder.HasData(
            new Department { Id = 1, DepartmentName = "Human Resources", Description = "Recruitment, employee relations, and benefits", CreatedAt = SeedDate },
            new Department { Id = 2, DepartmentName = "Information Technology", Description = "Software development and infrastructure", CreatedAt = SeedDate },
            new Department { Id = 3, DepartmentName = "Finance", Description = "Accounting, payroll, and budgeting", CreatedAt = SeedDate },
            new Department { Id = 4, DepartmentName = "Operations", Description = "Day-to-day business operations", CreatedAt = SeedDate },
            new Department { Id = 5, DepartmentName = "Sales", Description = "Customer acquisition and account management", CreatedAt = SeedDate });
    }
}