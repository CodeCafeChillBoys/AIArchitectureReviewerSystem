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

  const [workspaces, setWorkspaces] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Modal State
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');

  // 1. Fetch workspaces
  const fetchWorkspaces = async () => {
    const currentUserId = authService.getUserId();
    if (!currentUserId) {
      setError('Please log in to view your workspaces.');
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
      setError(err.response?.data?.message || err.message || 'Failed to load workspaces.');
      setWorkspaces([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchWorkspaces();
  }, []);

  // 2. Create workspace
  const handleCreateWorkspace = async (name, onSuccess) => {
    const currentUserId = authService.getUserId();
    if (!currentUserId) {
      setCreateError('Please log in to create a workspace.');
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
        setCreateError(res?.message || 'Failed to create workspace.');
      }
    } catch (err) {
      setCreateError(err.response?.data?.message || err.message || 'Error creating workspace.');
    } finally {
      setCreating(false);
    }
  };

  const [sortBy, setSortBy] = useState('newest');

  // Sort workspaces
  const sortedWorkspaces = [...workspaces].sort((a, b) => {
    if (sortBy === 'newest') {
      return new Date(b.createdAt || 0) - new Date(a.createdAt || 0);
    }
    if (sortBy === 'oldest') {
      return new Date(a.createdAt || 0) - new Date(b.createdAt || 0);
    }
    if (sortBy === 'name_asc') {
      return (a.name || '').localeCompare(b.name || '');
    }
    if (sortBy === 'name_desc') {
      return (b.name || '').localeCompare(a.name || '');
    }
    return 0;
  });

  return (
    <div style={{ padding: '32px 28px' }}>
      {/* 1. Header Toolbar */}
      <DashboardHeader
        totalCount={workspaces.length}
        sortBy={sortBy}
        onSortChange={setSortBy}
        onRefresh={fetchWorkspaces}
        loading={loading}
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
          <span>Loading workspaces from server...</span>
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
            Retry
          </button>
        </div>
      )}

      {/* 4. Workspace Grid or Empty State */}
      {!loading && (
        <WorkspaceGrid
          workspaces={sortedWorkspaces}
          onOpenCreateModal={() => setIsModalOpen(true)}
          onSelectWorkspace={(id) => navigate(`/workspace/${id}`)}
        />
      )}

      {/* 5. Create Workspace Modal */}
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
