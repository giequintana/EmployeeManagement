import { Link } from 'react-router';
import ErrorAlert from '@/components/common/ErrorAlert';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import PageHeader from '@/components/common/PageHeader';
import Pagination from '@/components/common/Pagination';
import EmployeeFilters from '@/components/employees/EmployeeFilters';
import EmployeeTable from '@/components/employees/EmployeeTable';
import { useApiQuery } from '@/hooks/useApiQuery';
import { useAuth } from '@/hooks/useAuth';
import { PAGE_SIZE_OPTIONS, useEmployeeQueryParams } from '@/hooks/useEmployeeQueryParams';
import { departmentService } from '@/services/departmentService';
import { employeeService } from '@/services/employeeService';

function EmployeeListPage() {
  const { isAdmin } = useAuth();
  const { query, updateQuery, resetQuery } = useEmployeeQueryParams();

  const { data: departments } = useApiQuery((signal) => departmentService.getAll(signal), []);
  const { data: result, error, isLoading, reload } = useApiQuery(
    (signal) => employeeService.getPaged(query, signal),
    [query],
  );

  const hasFilters = Boolean(query.search || query.departmentId || query.isActive);

  function handleSort(field) {
    const sortDescending = query.sortBy === field ? !query.sortDescending : false;
    updateQuery({ sortBy: field, sortDescending });
  }

  function handlePageChange(pageNumber) {
    updateQuery({ pageNumber }, { resetPage: false });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  const firstItem = result && result.totalCount > 0 ? (result.pageNumber - 1) * result.pageSize + 1 : 0;
  const lastItem = result ? firstItem + result.items.length - 1 : 0;

  return (
    <>
      <PageHeader
        title="Employees"
        subtitle={result ? `${result.totalCount.toLocaleString()} employees found` : 'Search, filter, and manage employees.'}
        actions={isAdmin && <Link to="/employees/new" className="btn btn-primary">Add employee</Link>}
      />

      <EmployeeFilters
        query={query}
        departments={departments ?? []}
        hasFilters={hasFilters}
        onFilterChange={(changes) => updateQuery(changes)}
        onSearch={(search) => updateQuery({ search }, { replace: true })}
        onReset={resetQuery}
      />

      <ErrorAlert error={error} onRetry={reload} />

      {isLoading && !result && <LoadingSpinner label="Loading employees..." />}

      {result && (
        <div className="card shadow-sm">
          <div className={isLoading ? 'opacity-50' : ''} aria-busy={isLoading}>
            {result.items.length > 0 ? (
              <EmployeeTable
                employees={result.items}
                sortBy={query.sortBy}
                sortDescending={query.sortDescending}
                onSort={handleSort}
                isAdmin={isAdmin}
              />
            ) : (
              <div className="text-center text-body-secondary py-5">
                {result.totalCount === 0 ? (
                  <>
                    <p className="mb-2">No employees match your filters.</p>
                    {hasFilters && (
                      <button type="button" className="btn btn-sm btn-outline-secondary" onClick={resetQuery}>
                        Reset filters
                      </button>
                    )}
                  </>
                ) : (
                  <>
                    <p className="mb-2">This page is empty.</p>
                    <button type="button" className="btn btn-sm btn-outline-secondary" onClick={() => handlePageChange(1)}>
                      Go to first page
                    </button>
                  </>
                )}
              </div>
            )}
          </div>

          <div className="card-footer d-flex flex-wrap justify-content-between align-items-center gap-2">
            <div className="d-flex align-items-center gap-2 small text-body-secondary">
              <span>
                {result.totalCount > 0 && result.items.length > 0
                  ? `Showing ${firstItem} to ${lastItem} of ${result.totalCount}`
                  : `${result.totalCount} total`}
              </span>
              <select
                className="form-select form-select-sm w-auto"
                aria-label="Rows per page"
                value={query.pageSize}
                onChange={(event) => updateQuery({ pageSize: Number(event.target.value) })}
              >
                {PAGE_SIZE_OPTIONS.map((size) => (
                  <option key={size} value={size}>{size} per page</option>
                ))}
              </select>
            </div>

            <Pagination
              pageNumber={result.pageNumber}
              totalPages={result.totalPages}
              onPageChange={handlePageChange}
            />
          </div>
        </div>
      )}
    </>
  );
}

export default EmployeeListPage;