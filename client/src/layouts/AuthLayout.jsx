import { Suspense } from 'react';
import { Outlet } from 'react-router';
import LoadingSpinner from '@/components/common/LoadingSpinner';

function AuthLayout() {
  return (
    <main className="container d-flex align-items-center justify-content-center min-vh-100 py-5">
      <div className="w-100" style={{ maxWidth: '26rem' }}>
        <Suspense fallback={<LoadingSpinner />}>
          <Outlet />
        </Suspense>
      </div>
    </main>
  );
}

export default AuthLayout;