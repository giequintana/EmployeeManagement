import { Link } from 'react-router';
import SortableHeader from '@/components/common/SortableHeader';
import { formatDate } from '@/utils/formatters';

const columns = [
  { label: 'Employee #', field: 'employeeNumber' },
  { label: 'Name', field: 'lastName' },
  { label: 'Email', className: 'd-none d-lg-table-cell' },
  { label: 'Department', field: 'department' },
  { label: 'Position', field: 'position', className: 'd-none d-md-table-cell' },
  { label: 'Hire date', field: 'hireDate', className: 'd-none d-sm-table-cell' },
  { label: 'Status' },
];

function EmployeeTable({ employees, sortBy, sortDescending, onSort, isAdmin }) {
  return (
    <div className="table-responsive">
      <table className="table table-hover align-middle mb-0">
        <thead className="table-light">
          <tr>
            {columns.map((column) =>
              column.field ? (
                <SortableHeader
                  key={column.label}
                  label={column.label}
                  field={column.field}
                  sortBy={sortBy}
                  sortDescending={sortDescending}
                  onSort={onSort}
                  className={column.className}
                />
              ) : (
                <th key={column.label} scope="col" className={column.className}>
                  {column.label}
                </th>
              ),
            )}
            {isAdmin && <th scope="col" className="text-end">Actions</th>}
          </tr>
        </thead>
        <tbody>
          {employees.map((employee) => (
            <tr key={employee.id}>
              <td className="text-nowrap">{employee.employeeNumber}</td>
              <td className="text-nowrap">
                <Link to={`/employees/${employee.id}`}>
                  {employee.lastName}, {employee.firstName}
                </Link>
              </td>
              <td className="d-none d-lg-table-cell">{employee.email}</td>
              <td>{employee.departmentName}</td>
              <td className="d-none d-md-table-cell">{employee.position}</td>
              <td className="d-none d-sm-table-cell text-nowrap">{formatDate(employee.hireDate)}</td>
              <td>
                <span className={`badge ${employee.isActive ? 'text-bg-success' : 'text-bg-secondary'}`}>
                  {employee.isActive ? 'Active' : 'Inactive'}
                </span>
              </td>
              {isAdmin && (
                <td className="text-end">
                  <Link to={`/employees/${employee.id}/edit`} className="btn btn-sm btn-outline-primary">
                    Edit
                  </Link>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default EmployeeTable;