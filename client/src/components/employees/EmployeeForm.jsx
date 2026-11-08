import FormField from '@/components/common/FormField';
import { useForm } from '@/hooks/useForm';
import {
  EMPLOYEE_ERROR_FIELDS,
  toEmployeePayload,
  validateEmployee,
} from '@/utils/employeeForm';
import { inputClass } from '@/utils/formErrors';

function EmployeeForm({ initialValues, departments, isEdit = false, submitLabel, onSubmit, onCancel }) {
  const { formRef, values, fieldErrors, formError, isSubmitting, handleChange, handleSubmit } = useForm({
    initialValues,
    validate: (formValues) => validateEmployee(formValues, { isEdit }),
    onSubmit: (formValues) => onSubmit(toEmployeePayload(formValues, { isEdit })),
    errorCodeFields: EMPLOYEE_ERROR_FIELDS,
  });

  const fieldProps = (name) => ({
    id: name,
    name,
    value: values[name],
    onChange: handleChange,
    disabled: isSubmitting,
  });

  return (
    <form ref={formRef} onSubmit={handleSubmit} noValidate className="card shadow-sm">
      <div className="card-body">
        {formError && <div className="alert alert-danger" role="alert">{formError}</div>}

        <h2 className="h6 text-uppercase text-body-secondary mb-3">Employment</h2>
        <div className="row g-3 mb-4">
          <FormField id="employeeNumber" label="Employee number" required={!isEdit} error={fieldErrors.employeeNumber}
            hint={isEdit ? 'Employee numbers cannot be changed.' : 'Letters, digits, and hyphens, for example EMP-1004.'}
            className="col-md-4">
            {isEdit ? (
              <input id="employeeNumber" className="form-control-plaintext fw-semibold" value={values.employeeNumber} readOnly />
            ) : (
              <input {...fieldProps('employeeNumber')} className={inputClass(fieldErrors.employeeNumber)} maxLength={20} autoFocus />
            )}
          </FormField>

          <FormField id="departmentId" label="Department" required error={fieldErrors.departmentId}
            hint={departments.length === 0 ? 'Create a department first.' : undefined} className="col-md-4">
            <select {...fieldProps('departmentId')} className={inputClass(fieldErrors.departmentId, 'form-select')}>
              <option value="">Select a department</option>
              {departments.map((department) => (
                <option key={department.id} value={String(department.id)}>{department.departmentName}</option>
              ))}
            </select>
          </FormField>

          <FormField id="position" label="Position" required error={fieldErrors.position} className="col-md-4">
            <input {...fieldProps('position')} className={inputClass(fieldErrors.position)} maxLength={100} />
          </FormField>

          <FormField id="hireDate" label="Hire date" required error={fieldErrors.hireDate} className="col-md-4">
            <input {...fieldProps('hireDate')} type="date" className={inputClass(fieldErrors.hireDate)} />
          </FormField>

          <FormField id="salary" label="Salary" required error={fieldErrors.salary} className="col-md-4">
            <input {...fieldProps('salary')} type="text" inputMode="decimal" className={inputClass(fieldErrors.salary)} placeholder="0.00" />
          </FormField>

          <div className="col-md-4 d-flex align-items-end">
            <div className="form-check form-switch mb-2">
              <input
                id="isActive"
                name="isActive"
                type="checkbox"
                role="switch"
                className="form-check-input"
                checked={values.isActive}
                onChange={handleChange}
                disabled={isSubmitting}
              />
              <label htmlFor="isActive" className="form-check-label">Active employee</label>
            </div>
          </div>
        </div>

        <h2 className="h6 text-uppercase text-body-secondary mb-3">Personal</h2>
        <div className="row g-3">
          <FormField id="firstName" label="First name" required error={fieldErrors.firstName} className="col-md-6">
            <input {...fieldProps('firstName')} className={inputClass(fieldErrors.firstName)} maxLength={100} autoComplete="off" />
          </FormField>

          <FormField id="lastName" label="Last name" required error={fieldErrors.lastName} className="col-md-6">
            <input {...fieldProps('lastName')} className={inputClass(fieldErrors.lastName)} maxLength={100} autoComplete="off" />
          </FormField>

          <FormField id="email" label="Email" required error={fieldErrors.email} className="col-md-6">
            <input {...fieldProps('email')} type="email" className={inputClass(fieldErrors.email)} maxLength={256} autoComplete="off" />
          </FormField>

          <FormField id="phoneNumber" label="Phone number" error={fieldErrors.phoneNumber} className="col-md-6">
            <input {...fieldProps('phoneNumber')} type="tel" className={inputClass(fieldErrors.phoneNumber)} maxLength={20} />
          </FormField>

          <FormField id="dateOfBirth" label="Date of birth" required error={fieldErrors.dateOfBirth} className="col-md-6">
            <input {...fieldProps('dateOfBirth')} type="date" className={inputClass(fieldErrors.dateOfBirth)} />
          </FormField>
        </div>
      </div>

      <div className="card-footer d-flex justify-content-end gap-2">
        <button type="button" className="btn btn-outline-secondary" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </button>
        <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
          {isSubmitting && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}
          {submitLabel}
        </button>
      </div>
    </form>
  );
}

export default EmployeeForm;