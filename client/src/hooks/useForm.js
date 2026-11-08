import { useRef, useState } from 'react';
import { toFormErrors } from '@/utils/formErrors';

export function useForm({ initialValues, validate, onSubmit, errorCodeFields = {} }) {
  const formRef = useRef(null);
  const [values, setValues] = useState(initialValues);
  const [fieldErrors, setFieldErrors] = useState({});
  const [formError, setFormError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  function focusFirstError() {
    requestAnimationFrame(() => formRef.current?.querySelector('.is-invalid')?.focus());
  }

  function handleChange(event) {
    const { name, type, value, checked } = event.target;
    setValues((current) => ({ ...current, [name]: type === 'checkbox' ? checked : value }));
    setFieldErrors((current) => ({ ...current, [name]: undefined }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setFormError('');

    const errors = validate(values);
    setFieldErrors(errors);

    if (Object.keys(errors).length > 0) {
      setFormError('Please fix the highlighted fields.');
      focusFirstError();
      return;
    }

    setIsSubmitting(true);

    try {
      await onSubmit(values);
      // On success the page navigates away.
    } catch (error) {
      const mapped = toFormErrors(error, errorCodeFields);
      setFieldErrors(mapped.fieldErrors);
      setFormError(mapped.message);
      setIsSubmitting(false);
      focusFirstError();
    }
  }

  return { formRef, values, fieldErrors, formError, isSubmitting, handleChange, handleSubmit };
}