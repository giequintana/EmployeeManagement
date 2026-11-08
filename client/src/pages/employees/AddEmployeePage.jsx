import { useNavigate } from 'react-router';
import ErrorAlert from '@/components/common/ErrorAlert';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import PageHeader from '@/components/common/PageHeader';
import EmployeeForm from '@/components/employees/EmployeeForm';
import { useApiQuery } from '@/hooks/useApiQuery';
import { departmentService } from '@/services/departmentService';
import { employeeService } from '@/services/employeeService';
import { emptyEmployeeValues } from '@/utils/employeeForm';

function AddEmployeePage() {
  const navigate = useNavigate();
  const { data: departments, error, reload } = useApiQuery((signal) => departmentService.getAll(signal), []);

  async function handleSubmit(payload) {
    const created = await employeeService.create(payload);
    navigate(`/employees/${created.id}`, {
      replace: true,
      state: { flash: `${created.firstName} ${created.lastName} was added.` },
    });
  }

  return (
    <>
      <PageHeader title="Add employee" />
      <ErrorAlert error={error} onRetry={reload} />
      {!departments && !error && <LoadingSpinner />}
      {departments && (
        <EmployeeForm
          initialValues={emptyEmployeeValues}
          departments={departments}
          submitLabel="Create employee"
          onSubmit={handleSubmit}
          onCancel={() => navigate('/employees')}
        />
      )}
    </>
  );
}

export default AddEmployeePage;