import FormField from '@/components/common/FormField';
import { useForm } from '@/hooks/useForm';
import {
  DEPARTMENT_ERROR_FIELDS,
  DESCRIPTION_MAX_LENGTH,
  toDepartmentPayload,
  validateDepartment,
} from '@/utils/departmentForm';
import { inputClass } from '@/utils/formErrors';

function DepartmentForm({ initialValues, submitLabel, onSubmit, onCancel }) {
  const { formRef, values, fieldErrors, formError, isSubmitting, handleChange, handleSubmit } = useForm({
    initialValues,
    validate: validateDepartment,
    onSubmit: (formValues) => onSubmit(toDepartmentPayload(formValues)),
    errorCodeFields: DEPARTMENT_ERROR_FIELDS,
  });

  return (
    <form ref={formRef} onSubmit={handleSubmit} noValidate className="card shadow-sm" style={{ maxWidth: '40rem' }}>
      <div className="card-body">
        {formError && <div className="alert alert-danger" role="alert">{formError}</div>}

        <div className="row g-3">
          <FormField id="departmentName" label="Department name" required error={fieldErrors.departmentName}>
            <input
              id="departmentName"
              name="departmentName"
              className={inputClass(fieldErrors.departmentName)}
              value={values.departmentName}
              onChange={handleChange}
              maxLength={100}
              disabled={isSubmitting}
              autoFocus
            />
          </FormField>

          <FormField
            id="description"
            label="Description"
            error={fieldErrors.description}
            hint={`${values.description.length} / ${DESCRIPTION_MAX_LENGTH} characters`}
          >
            <textarea
              id="description"
              name="description"
              rows={4}
              className={inputClass(fieldErrors.description)}
              value={values.description}
              onChange={handleChange}
              maxLength={DESCRIPTION_MAX_LENGTH}
              disabled={isSubmitting}
            />
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
export default DepartmentForm;