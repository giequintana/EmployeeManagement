export const DEPARTMENT_ERROR_FIELDS = {
  'Department.DuplicateName': 'departmentName',
};

export const DESCRIPTION_MAX_LENGTH = 500;

export const emptyDepartmentValues = {
  departmentName: '',
  description: '',
};

export function toDepartmentFormValues(department) {
  return {
    departmentName: department.departmentName,
    description: department.description ?? '',
  };
}

export function toDepartmentPayload(values) {
  return {
    departmentName: values.departmentName.trim(),
    description: values.description.trim() || null,
  };
}

export function validateDepartment(values) {
  const errors = {};
  const name = values.departmentName.trim();

  if (!name) {
    errors.departmentName = 'Department name is required.';
  } else if (name.length > 100) {
    errors.departmentName = 'Department name must be 100 characters or fewer.';
  }

  if (values.description.trim().length > DESCRIPTION_MAX_LENGTH) {
    errors.description = `Description must be ${DESCRIPTION_MAX_LENGTH} characters or fewer.`;
  }

  return errors;
}