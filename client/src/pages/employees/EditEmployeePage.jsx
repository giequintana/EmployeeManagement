import { useNavigate, useParams } from 'react-router';
import ErrorAlert from '@/components/common/ErrorAlert';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import PageHeader from '@/components/common/PageHeader';
import EmployeeForm from '@/components/employees/EmployeeForm';
import { useApiQuery } from '@/hooks/useApiQuery';
import NotFoundPage from '@/pages/NotFoundPage';
import { departmentService } from '@/services/departmentService';
import { employeeService } from '@/services/employeeService';
import { toEmployeeFormValues } from '@/utils/employeeForm';
import { parsePositiveId } from '@/utils/ids';

function EditEmployeePage() {
  const { id } = useParams();
  const employeeId = parsePositiveId(id);
  const navigate = useNavigate();

  const employeeQuery = useApiQuery(
    (signal) => (employeeId ? employeeService.getById(employeeId, signal) : Promise.resolve(null)),
    [employeeId],
  );
  const departmentsQuery = useApiQuery((signal) => departmentService.getAll(signal), []);

  if (!employeeId || employeeQuery.error?.status === 404) {
    return <NotFoundPage title="Employee not found" message="This employee does not exist or was deleted." />;
  }

  const employee = employeeQuery.data;
  const departments = departmentsQuery.data;
  const error = employeeQuery.error ?? departmentsQuery.error;

  async function handleSubmit(payload) {
    await employeeService.update(employeeId, payload);
    navigate(`/employees/${employeeId}`, { state: { flash: 'Employee updated.' } });
  }

  return (
    <>
      <PageHeader title={employee ? `Edit ${employee.firstName} ${employee.lastName}` : 'Edit employee'} />
      <ErrorAlert
        error={error}
        onRetry={() => {
          employeeQuery.reload();
          departmentsQuery.reload();
        }}
      />
      {(!employee || !departments) && !error && <LoadingSpinner />}
      {employee && departments && (
        <EmployeeForm
          key={employee.id}
          initialValues={toEmployeeFormValues(employee)}
          departments={departments}
          isEdit
          submitLabel="Save changes"
          onSubmit={handleSubmit}
          onCancel={() => navigate(`/employees/${employeeId}`)}
        />
      )}
    </>
  );
}

export default EditEmployeePage;