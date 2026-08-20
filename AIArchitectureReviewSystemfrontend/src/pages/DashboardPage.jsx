import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Loader2, AlertCircle } from 'lucide-react';
import { authService } from '../services/authService';
import { workspaceService } from '../services/workspaceService';

import DashboardHeader from '../components/dashboard/DashboardHeader';
import WorkspaceGrid from '../components/dashboard/WorkspaceGrid';
import CreateWorkspaceModal from '../components/dashboard/CreateWorkspaceModal';

export default function DashboardPage() {
  const navigate = useNavigate();

  // State danh sách Workspace và trạng thái tải
  const [workspaces, setWorkspaces] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  // State Modal Tạo Workspace
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');

  // 1. Tải danh sách Workspaces từ API
  const fetchWorkspaces = async () => {
    const currentUserId = authService.getUserId();
    if (!currentUserId) {
      setError('Vui lòng đăng nhập để xem danh sách Workspace.');
      setLoading(false);
      setWorkspaces([]);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const response = await workspaceService.getUserWorkspaces(currentUserId, 1, 30);

      if (response && response.success && response.data) {
        const items = response.data.items || response.data;
        setWorkspaces(Array.isArray(items) ? items : []);
      } else if (response && response.data) {
        const items = response.data.items || response.data;
        setWorkspaces(Array.isArray(items) ? items : []);
      } else if (Array.isArray(response)) {
        setWorkspaces(response);
      } else {
        setWorkspaces([]);
      }
    } catch (err) {
      console.error('API Workspaces error:', err);
      setError(err.response?.data?.message || err.message || 'Không thể tải danh sách Workspace.');
      setWorkspaces([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchWorkspaces();
  }, []);

  // 2. Tạo Workspace mới
  const handleCreateWorkspace = async (name, onSuccess) => {
    const currentUserId = authService.getUserId();
    if (!currentUserId) {
      setCreateError('Vui lòng đăng nhập để tạo Workspace.');
      return;
    }

    try {
      setCreating(true);
      setCreateError('');

      const res = await workspaceService.createWorkspace({
        name,
        userId: currentUserId,
      });

      if (res && (res.success || res.data)) {
        onSuccess?.();
        setIsModalOpen(false);
        await fetchWorkspaces();
      } else {
        setCreateError(res?.message || 'Không thể tạo workspace.');
      }
    } catch (err) {
      setCreateError(err.response?.data?.message || err.message || 'Lỗi khi gọi API tạo Workspace.');
    } finally {
      setCreating(false);
    }
  };

  // Lọc theo từ khóa tìm kiếm
  const filteredWorkspaces = workspaces.filter((ws) =>
    (ws.name || '').toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ padding: '32px 28px' }}>
      {/* 1. Header Toolbar (Tiêu đề, Search, Nút New) */}
      <DashboardHeader
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        onOpenCreateModal={() => {
          setCreateError('');
          setIsModalOpen(true);
        }}
      />

      {/* 2. Loading State */}
      {loading && (
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '60px 0',
          gap: '10px',
          color: 'var(--text-secondary)',
          fontSize: '14px',
        }}>
          <Loader2 size={20} className="animate-spin" color="var(--accent-primary)" />
          <span>Đang tải danh sách Workspaces từ server...</span>
        </div>
      )}

      {/* 3. Error State */}
      {!loading && error && (
        <div style={{
          padding: '16px',
          backgroundColor: 'var(--color-danger-bg)',
          border: '1px solid #fecaca',
          borderRadius: 'var(--radius-md)',
          color: 'var(--color-danger)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          marginBottom: '24px',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <AlertCircle size={18} />
            <span>{error}</span>
          </div>
          <button className="btn btn-secondary btn-sm" onClick={fetchWorkspaces}>
            Thử lại
          </button>
        </div>
      )}

      {/* 4. Danh sách Grid Workspace hoặc Empty State */}
      {!loading && (
        <WorkspaceGrid
          workspaces={filteredWorkspaces}
          searchTerm={searchTerm}
          onOpenCreateModal={() => setIsModalOpen(true)}
          onSelectWorkspace={(id) => navigate(`/workspace/${id}`)}
        />
      )}

      {/* 5. Modal Tạo Workspace */}
      <CreateWorkspaceModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onCreate={handleCreateWorkspace}
        creating={creating}
        error={createError}
      />
    </div>
  );
}
