using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException sqlException
                  && TryGetViolationKind(sqlException, out var kind))
        {
            throw new ConstraintViolationException(kind, exception);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private static bool TryGetViolationKind(SqlException exception, out ConstraintViolationKind kind)
    {
        switch (exception.Number)
        {
            case 2601: // Unique index violation
            case 2627: // Unique constraint or primary key violation
                kind = ConstraintViolationKind.Unique;
                return true;

            case 547 when exception.Message.Contains("CHECK constraint", StringComparison.OrdinalIgnoreCase):
                kind = ConstraintViolationKind.Check;
                return true;

            case 547: // Foreign key (REFERENCE / FOREIGN KEY) violation
                kind = ConstraintViolationKind.ForeignKey;
                return true;

            default:
                kind = default;
                return false;
        }
    }
}