import React, { useEffect } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './components/auth/ProtectedRoute';
import AppLayout from './components/layout/AppLayout';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import WorkspaceDetailPage from './pages/WorkspaceDetailPage';
import DiagramEditorPage from './pages/DiagramEditorPage';
import AIReviewCenterPage from './pages/AIReviewCenterPage';
import SettingsPage from './pages/SettingsPage';
import AdminPage from './pages/AdminPage';
import { initSignalR } from './services/signalrService';

export default function App() {
  useEffect(() => {
    initSignalR();
  }, []);

  return (
    <BrowserRouter>
      <Routes>
        {/* 1. Route công khai: Trang Đăng nhập */}
        <Route path="/login" element={<LoginPage />} />

        {/* 2. TOÀN BỘ CÁC ROUTE BẢO VỆ ĐỀU NẰM TRONG MỘT ROUTE CHA DUY NHẤT */}
        <Route element={<ProtectedRoute />}>
          <Route element={<AppLayout />}>
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/workspace/:id" element={<WorkspaceDetailPage />} />
            <Route path="/editor" element={<DiagramEditorPage />} />
            <Route path="/editor/:id" element={<DiagramEditorPage />} />
            <Route path="/review" element={<AIReviewCenterPage />} />
            <Route path="/review/:id" element={<AIReviewCenterPage />} />
            <Route path="/settings" element={<SettingsPage />} />
            <Route
              path="/admin"
              element={
                <ProtectedRoute requiredRole="Admin">
                  <AdminPage />
                </ProtectedRoute>
              }
            />
          </Route>
        </Route>

        {/* 3. Bất kỳ đường dẫn lạ nào khác -> chuyển hướng về /login */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
