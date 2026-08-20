import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import AppLayout from './components/layout/AppLayout';
import DashboardPage from './pages/DashboardPage';
import WorkspaceDetailPage from './pages/WorkspaceDetailPage';
import DiagramEditorPage from './pages/DiagramEditorPage';
import AIReviewCenterPage from './pages/AIReviewCenterPage';

export default function App() {
  return (
    <BrowserRouter>
      <AppLayout>
        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/workspace/:id" element={<WorkspaceDetailPage />} />
          <Route path="/editor" element={<DiagramEditorPage />} />
          <Route path="/editor/:id" element={<DiagramEditorPage />} />
          <Route path="/review" element={<AIReviewCenterPage />} />
          <Route path="/review/:id" element={<AIReviewCenterPage />} />
        </Routes>
      </AppLayout>
    </BrowserRouter>
  );
}
