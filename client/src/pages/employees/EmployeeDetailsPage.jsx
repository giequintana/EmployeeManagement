import { useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import ConfirmDialog from '@/components/common/ConfirmDialog';
import ErrorAlert from '@/components/common/ErrorAlert';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import PageHeader from '@/components/common/PageHeader';
import { useApiQuery } from '@/hooks/useApiQuery';
import { useAuth } from '@/hooks/useAuth';
import NotFoundPage from '@/pages/NotFoundPage';
import { employeeService } from '@/services/employeeService';
import { formatDate, formatDateTime, formatMoney } from '@/utils/formatters';
import { parsePositiveId } from '@/utils/ids';

function DetailItem({ label, children }) {
  return (
    <div className="col-sm-6 col-lg-4">
      <dt className="small text-body-secondary fw-normal">{label}</dt>
      <dd className="mb-0">{children}</dd>
    </div>
  );
}

function EmployeeDetailsPage() {
  const { id } = useParams();
  const employeeId = parsePositiveId(id);
  const navigate = useNavigate();
  const { isAdmin } = useAuth();

  const { data: employee, error, isLoading, reload } = useApiQuery(
    (signal) => (employeeId ? employeeService.getById(employeeId, signal) : Promise.resolve(null)),
    [employeeId],
  );

  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [deleteState, setDeleteState] = useState({ isDeleting: false, error: null });

  if (!employeeId || error?.status === 404) {
    return <NotFoundPage title="Employee not found" message="This employee does not exist or was deleted." />;
  }

  async function handleDelete() {
    setDeleteState({ isDeleting: true, error: null });

    try {
      await employeeService.remove(employee.id);
      navigate('/employees', {
        replace: true,
        state: { flash: `${employee.firstName} ${employee.lastName} was deleted.` },
      });
    } catch (deleteError) {
      setDeleteState({ isDeleting: false, error: deleteError });
    }
  }

  function closeDeleteDialog() {
    setShowDeleteDialog(false);
    setDeleteState({ isDeleting: false, error: null });
  }

  return (
    <>
      <div className="mb-3">
        <Link to="/employees" className="small">Back to employees</Link>
      </div>

      <ErrorAlert error={error} onRetry={reload} />
      {isLoading && !employee && <LoadingSpinner label="Loading employee..." />}

      {employee && (
        <>
          <PageHeader
            title={`${employee.firstName} ${employee.lastName}`}
            subtitle={
              <>
                {employee.employeeNumber}{' '}
                <span className={`badge ${employee.isActive ? 'text-bg-success' : 'text-bg-secondary'}`}>
                  {employee.isActive ? 'Active' : 'Inactive'}
                </span>
              </>
            }
            actions={
              isAdmin && (
                <>
                  <Link to={`/employees/${employee.id}/edit`} className="btn btn-outline-primary">Edit</Link>
                  <button type="button" className="btn btn-outline-danger" onClick={() => setShowDeleteDialog(true)}>
                    Delete
                  </button>
                </>
              )
            }
          />

          <div className="card shadow-sm">
            <div className="card-body">
              <dl className="row g-4 mb-0">
                <DetailItem label="Email"><a href={`mailto:${employee.email}`}>{employee.email}</a></DetailItem>
                <DetailItem label="Phone">{employee.phoneNumber || <span className="text-body-secondary">Not provided</span>}</DetailItem>
                <DetailItem label="Department">
                  <Link to={`/employees?departmentId=${employee.departmentId}`}>{employee.departmentName}</Link>
                </DetailItem>
                <DetailItem label="Position">{employee.position}</DetailItem>
                <DetailItem label="Hire date">{formatDate(employee.hireDate)}</DetailItem>
                <DetailItem label="Date of birth">{formatDate(employee.dateOfBirth)}</DetailItem>
                <DetailItem label="Salary">
                  {employee.salary == null
                    ? <span className="text-body-secondary">Restricted</span>
                    : formatMoney(employee.salary)}
                </DetailItem>
                <DetailItem label="Created">{formatDateTime(employee.createdAt)}</DetailItem>
                <DetailItem label="Last updated">
                  {employee.updatedAt ? formatDateTime(employee.updatedAt) : <span className="text-body-secondary">Never</span>}
                </DetailItem>
              </dl>
            </div>
          </div>

          <ConfirmDialog
            show={showDeleteDialog}
            title="Delete employee"
            message={`Delete ${employee.firstName} ${employee.lastName} (${employee.employeeNumber})? This cannot be undone. To keep their history, set them to Inactive instead.`}
            confirmLabel="Delete"
            isProcessing={deleteState.isDeleting}
            error={deleteState.error}
            onConfirm={handleDelete}
            onCancel={closeDeleteDialog}
          />
        </>
      )}
    </>
  );
}

export default EmployeeDetailsPage;