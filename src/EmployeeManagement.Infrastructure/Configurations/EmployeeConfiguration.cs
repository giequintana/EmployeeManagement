using EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees", table =>
        {
            table.HasCheckConstraint("CK_Employees_Salary_NonNegative", "[Salary] >= 0");
            table.HasCheckConstraint("CK_Employees_HireDate_AfterBirth", "[HireDate] > [DateOfBirth]");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EmployeeNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.Position)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Salary)
            .HasPrecision(18, 2);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true)
            .HasSentinel(true);

        builder.HasIndex(e => e.EmployeeNumber)
            .IsUnique();

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasIndex(e => new { e.LastName, e.FirstName });

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}