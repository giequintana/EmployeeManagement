import { Link } from 'react-router';

function percent(part, whole) {
  return whole === 0 ? 0 : (part / whole) * 100;
}

function DepartmentDistribution({ items, totalEmployees }) {
  if (items.length === 0) {
    return <p className="text-body-secondary mb-0">No departments yet.</p>;
  }

  const largest = Math.max(...items.map((item) => item.employeeCount), 1);

  return (
    <ul className="list-unstyled mb-0">
      {items.map((item) => {
        const inactive = item.employeeCount - item.activeEmployeeCount;
        const share = percent(item.employeeCount, totalEmployees);

        return (
          <li key={item.departmentId} className="mb-3">
            <div className="d-flex justify-content-between small mb-1">
              <Link to={`/employees?departmentId=${item.departmentId}`}>{item.departmentName}</Link>
              <span className="text-body-secondary">
                {item.employeeCount} ({share.toFixed(0)}%)
              </span>
            </div>

            <div className="progress-stacked" style={{ height: '0.75rem' }}>
              <div
                className="progress"
                role="progressbar"
                aria-label={`${item.departmentName} active employees`}
                aria-valuenow={item.activeEmployeeCount}
                aria-valuemin={0}
                aria-valuemax={largest}
                style={{ width: `${percent(item.activeEmployeeCount, largest)}%` }}
              >
                <div className="progress-bar bg-success" />
              </div>
              <div
                className="progress"
                role="progressbar"
                aria-label={`${item.departmentName} inactive employees`}
                aria-valuenow={inactive}
                aria-valuemin={0}
                aria-valuemax={largest}
                style={{ width: `${percent(inactive, largest)}%` }}
              >
                <div className="progress-bar bg-secondary" />
              </div>
            </div>
          </li>
        );
      })}
    </ul>
  );
}

export default DepartmentDistribution;