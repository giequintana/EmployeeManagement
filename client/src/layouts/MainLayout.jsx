import { Suspense, useState } from 'react';
import { Link, NavLink, Outlet, useLocation, useNavigate } from 'react-router';
import ErrorBoundary from '@/components/common/ErrorBoundary';
import FlashMessage from '@/components/common/FlashMessage';
import LoadingSpinner from '@/components/common/LoadingSpinner';
import { useAuth } from '@/hooks/useAuth';

const navItems = [
  { to: '/', label: 'Dashboard', end: true },
  { to: '/employees', label: 'Employees' },
  { to: '/departments', label: 'Departments' },
];

function MainLayout() {
  const { user, isAdmin, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const closeMenu = () => setIsMenuOpen(false);

  function handleSignOut() {
    closeMenu();
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-lg bg-dark" data-bs-theme="dark">
        <div className="container-xl">
          <Link className="navbar-brand fw-semibold" to="/" onClick={closeMenu}>
            Employee Management
          </Link>

          <button
            type="button"
            className="navbar-toggler"
            aria-controls="main-navbar"
            aria-expanded={isMenuOpen}
            aria-label="Toggle navigation"
            onClick={() => setIsMenuOpen((open) => !open)}
          >
            <span className="navbar-toggler-icon" />
          </button>

          <div id="main-navbar" className={`collapse navbar-collapse ${isMenuOpen ? 'show' : ''}`}>
            <ul className="navbar-nav me-auto">
              {navItems.map((item) => (
                <li key={item.to} className="nav-item">
                  <NavLink to={item.to} end={item.end} className="nav-link" onClick={closeMenu}>
                    {item.label}
                  </NavLink>
                </li>
              ))}
            </ul>

            <div className="d-flex align-items-center gap-3 py-2 py-lg-0">
              <span className="navbar-text">
                {user?.username}{' '}
                <span className={`badge ${isAdmin ? 'text-bg-danger' : 'text-bg-secondary'}`}>
                  {user?.role}
                </span>
              </span>
              <button type="button" className="btn btn-outline-light btn-sm" onClick={handleSignOut}>
                Sign out
              </button>
            </div>
          </div>
        </div>
      </nav>

    <main className="container-xl py-4 flex-grow-1">
        <FlashMessage />
        <ErrorBoundary key={location.pathname}>
            <Suspense fallback={<LoadingSpinner label="Loading page..." />}>
            <Outlet />
            </Suspense>
        </ErrorBoundary>
        </main>

      <footer className="border-top py-3 text-center text-body-secondary small">
        Employee Management System
      </footer>
    </div>
  );
}

export default MainLayout;