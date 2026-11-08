import { Link } from 'react-router';

function NotFoundPage({ title = 'Page not found', message = 'The page you are looking for does not exist.' }) {
  return (
    <div className="text-center py-5">
      <p className="display-6 fw-semibold mb-2">404</p>
      <h1 className="h4">{title}</h1>
      <p className="text-body-secondary">{message}</p>
      <Link to="/" className="btn btn-primary">Back to dashboard</Link>
    </div>
  );
}

export default NotFoundPage;