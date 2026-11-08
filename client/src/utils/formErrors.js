export function toFormErrors(error, errorCodeFields = {}) {
  if (error.isValidationError) {
    return {
      fieldErrors: Object.fromEntries(
        Object.entries(error.fieldErrors).map(([field, messages]) => [field, messages[0]]),
      ),
      message: 'Please fix the highlighted fields.',
    };
  }

  const field = errorCodeFields[error.errorCode];

  if (field) {
    return { fieldErrors: { [field]: error.message }, message: '' };
  }

  return { fieldErrors: {}, message: error.message };
}

export function inputClass(error, base = 'form-control') {
  return `${base} ${error ? 'is-invalid' : ''}`;
}