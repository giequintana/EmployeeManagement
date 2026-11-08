import { useLocation, useNavigate } from 'react-router';

function FlashMessage() {
  const location = useLocation();
  const navigate = useNavigate();
  const message = location.state?.flash;

  if (!message) {
    return null;
  }

  function dismiss() {
    navigate(`${location.pathname}${location.search}`, { replace: true, state: null });
  }

  return (
    <div className="alert alert-success alert-dismissible" role="status">
      {message}
      <button type="button" className="btn-close" aria-label="Close" onClick={dismiss} />
    </div>
  );
}

export default FlashMessage;