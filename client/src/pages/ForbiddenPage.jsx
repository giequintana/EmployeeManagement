import { Link } from 'react-router';

function ForbiddenPage() {
  return (
    <div className="text-center py-5">
      <p className="display-6 fw-semibold mb-2">403</p>
      <h1 className="h4">You do not have access to this page</h1>
      <p className="text-body-secondary">This action requires an administrator account.</p>
      <Link to="/" className="btn btn-primary">Back to dashboard</Link>
    </div>
  );
}

export default ForbiddenPage;