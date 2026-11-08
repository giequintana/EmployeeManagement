import { Link, useNavigate, useParams } from 'react-router';
import ErrorAlert from '@/components/common/ErrorAlert';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import PageHeader from '@/components/common/PageHeader';
import DepartmentForm from '@/components/departments/DepartmentForm';
import { useApiQuery } from '@/hooks/useApiQuery';
import NotFoundPage from '@/pages/NotFoundPage';
import { departmentService } from '@/services/departmentService';
import { toDepartmentFormValues } from '@/utils/departmentForm';
import { parsePositiveId } from '@/utils/ids';

function EditDepartmentPage() {
  const { id } = useParams();
  const departmentId = parsePositiveId(id);
  const navigate = useNavigate();

  const { data: department, error, isLoading, reload } = useApiQuery(
    (signal) => (departmentId ? departmentService.getById(departmentId, signal) : Promise.resolve(null)),
    [departmentId],
  );

  if (!departmentId || error?.status === 404) {
    return <NotFoundPage title="Department not found" message="This department does not exist or was deleted." />;
  }

  async function handleSubmit(payload) {
    const updated = await departmentService.update(departmentId, payload);
    navigate('/departments', { state: { flash: `${updated.departmentName} was updated.` } });
  }

  return (
    <>
      <PageHeader
        title={department ? `Edit ${department.departmentName}` : 'Edit department'}
        subtitle={
          department && (
            <Link to={`/employees?departmentId=${department.id}`}>
              {department.employeeCount} {department.employeeCount === 1 ? 'employee' : 'employees'}
            </Link>
          )
        }
      />
      <ErrorAlert error={error} onRetry={reload} />
      {isLoading && !department && <LoadingSpinner />}
      {department && (
        <DepartmentForm
          key={department.id}
          initialValues={toDepartmentFormValues(department)}
          submitLabel="Save changes"
          onSubmit={handleSubmit}
          onCancel={() => navigate('/departments')}
        />
      )}
    </>
  );
}

export default EditDepartmentPage;