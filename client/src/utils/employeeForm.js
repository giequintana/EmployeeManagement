const EMPLOYEE_NUMBER_PATTERN = /^[A-Za-z0-9-]+$/;
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+$/;
const PHONE_PATTERN = /^[0-9+\-\s()]+$/;
const SALARY_PATTERN = /^\d+(\.\d{1,2})?$/;
const MAX_SALARY = 10_000_000;

export const EMPLOYEE_ERROR_FIELDS = {
  'Employee.DuplicateEmployeeNumber': 'employeeNumber',
  'Employee.DuplicateEmail': 'email',
  'Employee.DepartmentNotFound': 'departmentId',
};

export const emptyEmployeeValues = {
  employeeNumber: '',
  firstName: '',
  lastName: '',
  email: '',
  phoneNumber: '',
  dateOfBirth: '',
  hireDate: '',
  position: '',
  salary: '',
  departmentId: '',
  isActive: true,
};

function todayIso() {
  const now = new Date();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  return `${now.getFullYear()}-${month}-${day}`;
}

function addYears(isoDate, years) {
  const [year, month, day] = isoDate.split('-');
  return `${Number(year) + years}-${month}-${day}`;
}

function checkRequiredText(errors, values, field, label, maxLength) {
  const value = values[field].trim();

  if (!value) {
    errors[field] = `${label} is required.`;
  } else if (value.length > maxLength) {
    errors[field] = `${label} must be ${maxLength} characters or fewer.`;
  }
}

export function toEmployeeFormValues(employee) {
  return {
    employeeNumber: employee.employeeNumber,
    firstName: employee.firstName,
    lastName: employee.lastName,
    email: employee.email,
    phoneNumber: employee.phoneNumber ?? '',
    dateOfBirth: employee.dateOfBirth,
    hireDate: employee.hireDate,
    position: employee.position,
    salary: employee.salary == null ? '' : String(employee.salary),
    departmentId: String(employee.departmentId),
    isActive: employee.isActive,
  };
}

export function toEmployeePayload(values, { isEdit = false } = {}) {
  const payload = {
    firstName: values.firstName.trim(),
    lastName: values.lastName.trim(),
    email: values.email.trim(),
    phoneNumber: values.phoneNumber.trim() || null,
    dateOfBirth: values.dateOfBirth,
    hireDate: values.hireDate,
    position: values.position.trim(),
    salary: Number(values.salary),
    departmentId: Number(values.departmentId),
    isActive: values.isActive,
  };

  return isEdit ? payload : { ...payload, employeeNumber: values.employeeNumber.trim() };
}

export function validateEmployee(values, { isEdit = false } = {}) {
  const errors = {};
  const today = todayIso();

  if (!isEdit) {
    const employeeNumber = values.employeeNumber.trim();

    if (!employeeNumber) {
      errors.employeeNumber = 'Employee number is required.';
    } else if (employeeNumber.length > 20) {
      errors.employeeNumber = 'Employee number must be 20 characters or fewer.';
    } else if (!EMPLOYEE_NUMBER_PATTERN.test(employeeNumber)) {
      errors.employeeNumber = 'Use letters, digits, and hyphens only.';
    }
  }

  checkRequiredText(errors, values, 'firstName', 'First name', 100);
  checkRequiredText(errors, values, 'lastName', 'Last name', 100);
  checkRequiredText(errors, values, 'position', 'Position', 100);

  const email = values.email.trim();
  if (!email) {
    errors.email = 'Email is required.';
  } else if (email.length > 256) {
    errors.email = 'Email must be 256 characters or fewer.';
  } else if (!EMAIL_PATTERN.test(email)) {
    errors.email = 'Enter a valid email address.';
  }

  const phone = values.phoneNumber.trim();
  if (phone && (phone.length > 20 || !PHONE_PATTERN.test(phone))) {
    errors.phoneNumber = 'Use up to 20 digits, spaces, and + - ( ).';
  }

  if (!values.dateOfBirth) {
    errors.dateOfBirth = 'Date of birth is required.';
  } else if (values.dateOfBirth >= today) {
    errors.dateOfBirth = 'Date of birth must be in the past.';
  } else if (values.dateOfBirth <= addYears(today, -100)) {
    errors.dateOfBirth = 'Date of birth cannot be more than 100 years ago.';
  }

  if (!values.hireDate) {
    errors.hireDate = 'Hire date is required.';
  } else if (values.hireDate > addYears(today, 1)) {
    errors.hireDate = 'Hire date cannot be more than one year in the future.';
  } else if (values.dateOfBirth && !errors.dateOfBirth && values.hireDate < addYears(values.dateOfBirth, 18)) {
    errors.hireDate = 'Employee must be at least 18 years old on the hire date.';
  }

  const salary = String(values.salary).trim();
  if (!salary) {
    errors.salary = 'Salary is required.';
  } else if (!SALARY_PATTERN.test(salary)) {
    errors.salary = 'Enter a positive amount with up to 2 decimals.';
  } else if (Number(salary) > MAX_SALARY) {
    errors.salary = 'Salary cannot exceed 10,000,000.';
  }

  if (!values.departmentId) {
    errors.departmentId = 'Select a department.';
  }

  return errors;
}