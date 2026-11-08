import { lazy } from 'react';
import { Route, Routes } from 'react-router';
import AuthLayout from '@/layouts/AuthLayout';
import MainLayout from '@/layouts/MainLayout';
import NotFoundPage from '@/pages/NotFoundPage';
import ProtectedRoute from '@/routes/ProtectedRoute';

const LoginPage = lazy(() => import('@/pages/LoginPage'));
const DashboardPage = lazy(() => import('@/pages/DashboardPage'));
const EmployeeListPage = lazy(() => import('@/pages/employees/EmployeeListPage'));
const EmployeeDetailsPage = lazy(() => import('@/pages/employees/EmployeeDetailsPage'));
const AddEmployeePage = lazy(() => import('@/pages/employees/AddEmployeePage'));
const EditEmployeePage = lazy(() => import('@/pages/employees/EditEmployeePage'));
const DepartmentListPage = lazy(() => import('@/pages/departments/DepartmentListPage'));
const AddDepartmentPage = lazy(() => import('@/pages/departments/AddDepartmentPage'));
const EditDepartmentPage = lazy(() => import('@/pages/departments/EditDepartmentPage'));

function AppRoutes() {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
      </Route>

      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="/employees" element={<EmployeeListPage />} />
          <Route path="/employees/:id" element={<EmployeeDetailsPage />} />
          <Route path="/departments" element={<DepartmentListPage />} />

          <Route element={<ProtectedRoute requireAdmin />}>
            <Route path="/employees/new" element={<AddEmployeePage />} />
            <Route path="/employees/:id/edit" element={<EditEmployeePage />} />
            <Route path="/departments/new" element={<AddDepartmentPage />} />
            <Route path="/departments/:id/edit" element={<EditDepartmentPage />} />
          </Route>

          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Route>
    </Routes>
  );
}

export default AppRoutes;