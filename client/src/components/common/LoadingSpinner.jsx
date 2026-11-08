function LoadingSpinner({ label = 'Loading...' }) {
  return (
    <div className="d-flex justify-content-center align-items-center gap-2 py-5 text-body-secondary" role="status">
      <span className="spinner-border spinner-border-sm" aria-hidden="true" />
      <span>{label}</span>
    </div>
  );
}

export default LoadingSpinner;