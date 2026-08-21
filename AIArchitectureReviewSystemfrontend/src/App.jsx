import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import AppLayout from './components/layout/AppLayout';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import WorkspaceDetailPage from './pages/WorkspaceDetailPage';
import DiagramEditorPage from './pages/DiagramEditorPage';
import AIReviewCenterPage from './pages/AIReviewCenterPage';
import SettingsPage from './pages/SettingsPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* 1. Mặc định khi mở web vào "/" -> Chuyển hướng ngay sang /login */}
        <Route path="/" element={<Navigate to="/login" replace />} />

        {/* 2. Trang Login độc lập (Không có Sidebar & Header) */}
        <Route path="/login" element={<LoginPage />} />

        {/* 3. Các trang làm việc sau khi đăng nhập (Được bọc trong AppLayout) */}
        <Route
          path="/*"
          element={
            <AppLayout>
              <Routes>
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/workspace/:id" element={<WorkspaceDetailPage />} />
                <Route path="/editor" element={<DiagramEditorPage />} />
                <Route path="/editor/:id" element={<DiagramEditorPage />} />
                <Route path="/review" element={<AIReviewCenterPage />} />
                <Route path="/review/:id" element={<AIReviewCenterPage />} />
                <Route path="/settings" element={<SettingsPage />} />
                <Route path="*" element={<Navigate to="/login" replace />} />
              </Routes>
            </AppLayout>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}
