import { Component } from 'react';

class ErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { error: null };
  }

  static getDerivedStateFromError(error) {
    return { error };
  }

  componentDidCatch(error, info) {
    // In production this would go to a logging service (Application Insights, Sentry).
    console.error('Unhandled UI error', error, info.componentStack);
  }

  reset = () => {
    this.setState({ error: null });
  };

  render() {
    const { error } = this.state;

    if (!error) {
      return this.props.children;
    }

    return (
      <div className="text-center py-5" role="alert">
        <p className="display-6 fw-semibold mb-2">Something went wrong</p>
        <p className="text-body-secondary">
          This page hit an unexpected problem. Try again, or go back to the dashboard.
        </p>
        {import.meta.env.DEV && (
          <pre className="text-start small bg-body-secondary p-3 rounded mx-auto" style={{ maxWidth: '40rem', whiteSpace: 'pre-wrap' }}>
            {error.message}
          </pre>
        )}
        <div className="d-flex justify-content-center gap-2">
          <button type="button" className="btn btn-primary" onClick={this.reset}>
            Try again
          </button>
          <a href="/" className="btn btn-outline-secondary">Go to dashboard</a>
        </div>
      </div>
    );
  }
}

export default ErrorBoundary;