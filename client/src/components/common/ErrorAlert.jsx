function ErrorAlert({ error, onRetry }) {
  if (!error) {
    return null;
  }

  return (
    <div className="alert alert-danger d-flex flex-wrap justify-content-between align-items-center gap-2" role="alert">
      <div>
        <div>{error.message}</div>
        {error.traceId && <div className="small opacity-75">Reference: {error.traceId}</div>}
      </div>
      {onRetry && (
        <button type="button" className="btn btn-sm btn-outline-danger" onClick={onRetry}>
          Try again
        </button>
      )}
    </div>
  );
}

export default ErrorAlert;