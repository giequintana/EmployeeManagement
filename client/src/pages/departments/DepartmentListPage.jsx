import { useState } from 'react';
import { Link, useNavigate } from 'react-router';
import ConfirmDialog from '@/components/common/ConfirmDialog';
import ErrorAlert from '@/components/common/ErrorAlert';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import PageHeader from '@/components/common/PageHeader';
import { useApiQuery } from '@/hooks/useApiQuery';
import { useAuth } from '@/hooks/useAuth';
import { departmentService } from '@/services/departmentService';
import { formatDateTime } from '@/utils/formatters';

const initialDeleteState = { isDeleting: false, error: null };

function DepartmentListPage() {
  const { isAdmin } = useAuth();
  const navigate = useNavigate();

  const { data: departments, error, isLoading, reload } = useApiQuery(
    (signal) => departmentService.getAll(signal),
    [],
  );

  const [pendingDelete, setPendingDelete] = useState(null);
  const [deleteState, setDeleteState] = useState(initialDeleteState);
  const [blockedDepartment, setBlockedDepartment] = useState(null);

  function openDeleteDialog(department) {
    setBlockedDepartment(null);
    setDeleteState(initialDeleteState);
    setPendingDelete(department);
  }

  function closeDeleteDialog() {
    setPendingDelete(null);
    setDeleteState(initialDeleteState);
  }

  async function handleDelete() {
    const department = pendingDelete;
    setDeleteState({ isDeleting: true, error: null });

    try {
      await departmentService.remove(department.id);
      closeDeleteDialog();
      navigate('/departments', { replace: true, state: { flash: `${department.departmentName} was deleted.` } });
      reload();
    } catch (deleteError) {
      if (deleteError.errorCode === 'Department.HasEmployees') {
        closeDeleteDialog();
        setBlockedDepartment(department);
        reload();
      } else {
        setDeleteState({ isDeleting: false, error: deleteError });
      }
    }
  }

  return (
    <>
      <PageHeader
        title="Departments"
        subtitle={departments ? `${departments.length} departments` : undefined}
        actions={isAdmin && <Link to="/departments/new" className="btn btn-primary">Add department</Link>}
      />

      {blockedDepartment && (
        <div className="alert alert-warning alert-dismissible" role="alert">
          <strong>{blockedDepartment.departmentName}</strong> still has employees, so it cannot be deleted.
          Reassign them to another department or remove them first.
          <div className="mt-2">
            <Link to={`/employees?departmentId=${blockedDepartment.id}`} className="alert-link">
              View its employees
            </Link>
          </div>
          <button type="button" className="btn-close" aria-label="Close" onClick={() => setBlockedDepartment(null)} />
        </div>
      )}

      <ErrorAlert error={error} onRetry={reload} />
      {isLoading && !departments && <LoadingSpinner label="Loading departments..." />}

      {departments && departments.length === 0 && (
        <div className="card shadow-sm">
          <div className="card-body text-center text-body-secondary py-5">
            <p className="mb-2">No departments yet.</p>
            {isAdmin && <Link to="/departments/new" className="btn btn-sm btn-primary">Add the first department</Link>}
          </div>
        </div>
      )}

      {departments && departments.length > 0 && (
        <div className={`card shadow-sm ${isLoading ? 'opacity-50' : ''}`}>
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th scope="col">Department</th>
                  <th scope="col" className="d-none d-md-table-cell">Description</th>
                  <th scope="col" className="text-end">Employees</th>
                  <th scope="col" className="d-none d-lg-table-cell">Last updated</th>
                  {isAdmin && <th scope="col" className="text-end">Actions</th>}
                </tr>
              </thead>
              <tbody>
                {departments.map((department) => {
                  const hasEmployees = department.employeeCount > 0;

                  return (
                    <tr key={department.id}>
                      <td className="fw-semibold">{department.departmentName}</td>
                      <td className="d-none d-md-table-cell text-body-secondary">
                        {department.description || <span className="fst-italic">No description</span>}
                      </td>
                      <td className="text-end">
                        <Link to={`/employees?departmentId=${department.id}`}>{department.employeeCount}</Link>
                      </td>
                      <td className="d-none d-lg-table-cell text-nowrap">
                        {formatDateTime(department.updatedAt ?? department.createdAt)}
                      </td>
                      {isAdmin && (
                        <td className="text-end text-nowrap">
                          <Link to={`/departments/${department.id}/edit`} className="btn btn-sm btn-outline-primary me-2">
                            Edit
                          </Link>
                          <span
                            className="d-inline-block"
                            tabIndex={hasEmployees ? 0 : undefined}
                            title={hasEmployees ? `Reassign or remove its ${department.employeeCount} employees first.` : undefined}
                          >
                            <button
                              type="button"
                              className="btn btn-sm btn-outline-danger"
                              onClick={() => openDeleteDialog(department)}
                              disabled={hasEmployees}
                            >
                              Delete
                            </button>
                          </span>
                        </td>
                      )}
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      )}

      <ConfirmDialog
        show={pendingDelete !== null}
        title="Delete department"
        message={pendingDelete ? `Delete the ${pendingDelete.departmentName} department? This cannot be undone.` : ''}
        confirmLabel="Delete"
        isProcessing={deleteState.isDeleting}
        error={deleteState.error}
        onConfirm={handleDelete}
        onCancel={closeDeleteDialog}
      />
    </>
  );
}

export default DepartmentListPage;