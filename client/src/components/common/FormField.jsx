function FormField({ id, label, error, hint, required = false, className = 'col-12', children }) {
  return (
    <div className={className}>
      <label htmlFor={id} className="form-label">
        {label}
        {required && <span className="text-danger ms-1" aria-hidden="true">*</span>}
      </label>
      {children}
      {error ? (
        <div className="invalid-feedback d-block">{error}</div>
      ) : (
        hint && <div className="form-text">{hint}</div>
      )}
    </div>
  );
}

export default FormField;