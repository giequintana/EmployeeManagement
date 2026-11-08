import { Link } from 'react-router';
import ErrorAlert from '@/components/common/ErrorAlert';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import PageHeader from '@/components/common/PageHeader';
import DepartmentDistribution from '@/components/dashboard/DepartmentDistribution';
import StatCard from '@/components/dashboard/StatCard';
import { useApiQuery } from '@/hooks/useApiQuery';
import { useAuth } from '@/hooks/useAuth';
import { dashboardService } from '@/services/dashboardService';

function DashboardPage() {
    if (new URLSearchParams(window.location.search).has('crash')) throw new Error('Test crash from DashboardPage');
  const { user, isAdmin } = useAuth();
  const { data: summary, error, isLoading, reload } = useApiQuery(
    (signal) => dashboardService.getSummary(signal),
    [],
  );

  const activeShare = summary && summary.totalEmployees > 0
    ? Math.round((summary.activeEmployees / summary.totalEmployees) * 100)
    : 0;

  return (
    <>
      <PageHeader
        title="Dashboard"
        subtitle={`Welcome back, ${user.username}.`}
        actions={
          <>
            <button type="button" className="btn btn-outline-secondary" onClick={reload} disabled={isLoading}>
              {isLoading && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}
              Refresh
            </button>
            {isAdmin && <Link to="/employees/new" className="btn btn-primary">Add employee</Link>}
          </>
        }
      />

      <ErrorAlert error={error} onRetry={reload} />

      {isLoading && !summary && <LoadingSpinner label="Loading dashboard..." />}

      {summary && (
        <>
          <div className="row row-cols-1 row-cols-sm-2 row-cols-xl-4 g-3 mb-4">
            <div className="col">
              <StatCard label="Total employees" value={summary.totalEmployees} />
            </div>
            <div className="col">
              <StatCard
                label="Active"
                value={summary.activeEmployees}
                variant="success"
                hint={`${activeShare}% of all employees`}
              />
            </div>
            <div className="col">
              <StatCard label="Inactive" value={summary.inactiveEmployees} variant="secondary" />
            </div>
            <div className="col">
              <StatCard label="Departments" value={summary.totalDepartments} variant="info" />
            </div>
          </div>

          <div className="card shadow-sm">
            <div className="card-header d-flex flex-wrap justify-content-between align-items-center gap-2">
              <span className="fw-semibold">Employees by department</span>
              <span className="small text-body-secondary">
                <span className="badge text-bg-success me-1">&nbsp;</span>Active
                <span className="badge text-bg-secondary ms-3 me-1">&nbsp;</span>Inactive
              </span>
            </div>
            <div className="card-body">
              <DepartmentDistribution
                items={summary.employeesByDepartment}
                totalEmployees={summary.totalEmployees}
              />
            </div>
          </div>
        </>
      )}
    </>
  );
}

export default DashboardPage;