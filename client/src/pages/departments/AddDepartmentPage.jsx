import { useNavigate } from 'react-router';
import PageHeader from '@/components/common/PageHeader';
import DepartmentForm from '@/components/departments/DepartmentForm';
import { departmentService } from '@/services/departmentService';
import { emptyDepartmentValues } from '@/utils/departmentForm';

function AddDepartmentPage() {
  const navigate = useNavigate();

  async function handleSubmit(payload) {
    const created = await departmentService.create(payload);
    navigate('/departments', {
      replace: true,
      state: { flash: `${created.departmentName} was added.` },
    });
  }

  return (
    <>
      <PageHeader title="Add department" />
      <DepartmentForm
        initialValues={emptyDepartmentValues}
        submitLabel="Create department"
        onSubmit={handleSubmit}
        onCancel={() => navigate('/departments')}
      />
    </>
  );
}

export default AddDepartmentPage;