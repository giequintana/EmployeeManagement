import { useEffect, useLayoutEffect, useRef } from 'react';

function ConfirmDialog({
  show,
  title,
  message,
  confirmLabel = 'Confirm',
  confirmVariant = 'danger',
  isProcessing = false,
  error = null,
  onConfirm,
  onCancel,
}) {
  const cancelButtonRef = useRef(null);
  const latest = useRef({ onCancel, isProcessing });

  useLayoutEffect(() => {
    latest.current = { onCancel, isProcessing };
  });

  useEffect(() => {
    if (!show) {
      return undefined;
    }

    cancelButtonRef.current?.focus();
    document.body.classList.add('modal-open');

    function handleKeyDown(event) {
      if (event.key === 'Escape' && !latest.current.isProcessing) {
        latest.current.onCancel();
      }
    }

    document.addEventListener('keydown', handleKeyDown);

    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.body.classList.remove('modal-open');
    };
  }, [show]);

  if (!show) {
    return null;
  }

  return (
    <>
      <div className="modal d-block" tabIndex={-1} role="dialog" aria-modal="true" aria-labelledby="confirm-dialog-title">
        <div className="modal-dialog modal-dialog-centered">
          <div className="modal-content">
            <div className="modal-header">
              <h2 id="confirm-dialog-title" className="modal-title h5">{title}</h2>
              <button type="button" className="btn-close" aria-label="Close" onClick={onCancel} disabled={isProcessing} />
            </div>
            <div className="modal-body">
              <p className="mb-0">{message}</p>
              {error && <div className="alert alert-danger mt-3 mb-0">{error.message}</div>}
            </div>
            <div className="modal-footer">
              <button ref={cancelButtonRef} type="button" className="btn btn-outline-secondary" onClick={onCancel} disabled={isProcessing}>
                Cancel
              </button>
              <button type="button" className={`btn btn-${confirmVariant}`} onClick={onConfirm} disabled={isProcessing}>
                {isProcessing && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}
                {confirmLabel}
              </button>
            </div>
          </div>
        </div>
      </div>
      <div className="modal-backdrop show" />
    </>
  );
}

export default ConfirmDialog;