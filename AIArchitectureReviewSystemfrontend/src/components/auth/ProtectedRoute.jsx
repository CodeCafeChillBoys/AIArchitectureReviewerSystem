import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { authService } from '../../services/authService';

/**
 * Route Guard cho React Router.
 * Hỗ trợ bọc theo kiểu Route cha với <Outlet /> hoặc bọc trực tiếp {children}.
 * Hỗ trợ kiểm tra quyền hạn (Role) ví dụ requiredRole="Admin".
 */
export default function ProtectedRoute({ children, requiredRole }) {
  const location = useLocation();
  const isAuth = authService.isAuthenticated();

  if (!isAuth) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Kiểm tra quyền nếu có yêu cầu role cụ thể
  if (requiredRole === 'Admin' && !authService.isAdmin()) {
    return <Navigate to="/dashboard" replace />;
  }

  return children ? children : <Outlet />;
}


