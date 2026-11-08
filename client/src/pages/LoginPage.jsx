import { useState } from 'react';
import { Navigate, useLocation } from 'react-router';
import { useAuth } from '@/hooks/useAuth';

const initialForm = { username: '', password: '' };

function validate(form) {
  const errors = {};

  if (!form.username.trim()) {
    errors.username = 'Username is required.';
  }

  if (!form.password) {
    errors.password = 'Password is required.';
  }

  return errors;
}

function LoginPage() {
  const { isAuthenticated, login, sessionExpired } = useAuth();
  const location = useLocation();

  const [form, setForm] = useState(initialForm);
  const [fieldErrors, setFieldErrors] = useState({});
  const [errorMessage, setErrorMessage] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const from = location.state?.from;
  const redirectTo = from ? `${from.pathname}${from.search}` : '/';


  if (isAuthenticated) {
    return <Navigate to={redirectTo} replace />;
  }

  function handleChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
    setFieldErrors((current) => ({ ...current, [name]: undefined }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setErrorMessage('');

    const errors = validate(form);
    setFieldErrors(errors);

    if (Object.keys(errors).length > 0) {
      return;
    }

    setIsSubmitting(true);

    try {
      await login(form.username.trim(), form.password);
      // On success, the provider's state changes, this page re-renders,
      // and the isAuthenticated check above redirects.
    } catch (error) {
      if (error.isValidationError) {
        setFieldErrors(
          Object.fromEntries(
            Object.entries(error.fieldErrors).map(([field, messages]) => [field, messages[0]]),
          ),
        );
      } else {
        setErrorMessage(error.message);
      }

      setForm((current) => ({ ...current, password: '' }));
      setIsSubmitting(false);
    }
  }

  return (
   <div className="card shadow-sm">
        <div className="card-body p-4">
          <h1 className="h4 mb-1">Employee Management</h1>
          <p className="text-body-secondary mb-4">Sign in to continue</p>

          {sessionExpired && !errorMessage && (
            <div className="alert alert-info" role="status">
              Your session has expired. Please sign in again.
            </div>
          )}

          {errorMessage && (
            <div className="alert alert-danger" role="alert">
              {errorMessage}
            </div>
          )}

          <form onSubmit={handleSubmit} noValidate>
            <div className="mb-3">
              <label htmlFor="username" className="form-label">Username</label>
              <input
                id="username"
                name="username"
                type="text"
                className={`form-control ${fieldErrors.username ? 'is-invalid' : ''}`}
                value={form.username}
                onChange={handleChange}
                autoComplete="username"
                autoFocus
                disabled={isSubmitting}
              />
              {fieldErrors.username && <div className="invalid-feedback">{fieldErrors.username}</div>}
            </div>

            <div className="mb-4">
              <label htmlFor="password" className="form-label">Password</label>
              <input
                id="password"
                name="password"
                type="password"
                className={`form-control ${fieldErrors.password ? 'is-invalid' : ''}`}
                value={form.password}
                onChange={handleChange}
                autoComplete="current-password"
                disabled={isSubmitting}
              />
              {fieldErrors.password && <div className="invalid-feedback">{fieldErrors.password}</div>}
            </div>

            <button type="submit" className="btn btn-primary w-100" disabled={isSubmitting}>
              {isSubmitting && (
                <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />
              )}
              {isSubmitting ? 'Signing in...' : 'Sign in'}
            </button>
          </form>
        </div>
    </div>
  );
}

export default LoginPage;