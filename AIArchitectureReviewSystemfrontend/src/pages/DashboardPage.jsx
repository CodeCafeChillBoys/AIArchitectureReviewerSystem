import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Loader2, AlertCircle } from 'lucide-react';
import { authService } from '../services/authService';
import { workspaceService } from '../services/workspaceService';

import DashboardHeader from '../components/dashboard/DashboardHeader';
import WorkspaceGrid from '../components/dashboard/WorkspaceGrid';
import CreateWorkspaceModal from '../components/dashboard/CreateWorkspaceModal';
import Pagination from '../components/common/Pagination';

export default function DashboardPage() {
  const navigate = useNavigate();

  const [workspaces, setWorkspaces] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Pagination states
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(8);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  // Modal State
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');

  // 1. Fetch workspaces with pagination
  const fetchWorkspaces = useCallback(async (page = pageIndex, size = pageSize) => {
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

      const response = await workspaceService.getUserWorkspaces(currentUserId, page, size);

      if (response && response.success && response.data) {
        const data = response.data;
        const items = data.items || data;
        setWorkspaces(Array.isArray(items) ? items : []);
        setTotalCount(data.totalCount ?? (Array.isArray(items) ? items.length : 0));
        setTotalPages(data.totalPages ?? Math.max(1, Math.ceil((data.totalCount || items.length || 0) / size)));
      } else if (response && response.data) {
        const data = response.data;
        const items = data.items || data;
        setWorkspaces(Array.isArray(items) ? items : []);
        setTotalCount(data.totalCount ?? (Array.isArray(items) ? items.length : 0));
        setTotalPages(data.totalPages ?? Math.max(1, Math.ceil((data.totalCount || items.length || 0) / size)));
      } else if (Array.isArray(response)) {
        setWorkspaces(response);
        setTotalCount(response.length);
        setTotalPages(Math.max(1, Math.ceil(response.length / size)));
      } else {
        setWorkspaces([]);
        setTotalCount(0);
        setTotalPages(1);
      }
    } catch (err) {
      console.error('API Workspaces error:', err);
      setError(err.response?.data?.message || err.message || 'Failed to load workspaces.');
      setWorkspaces([]);
    } finally {
      setLoading(false);
    }
  }, [pageIndex, pageSize]);

  useEffect(() => {
    fetchWorkspaces(pageIndex, pageSize);
  }, [pageIndex, pageSize, fetchWorkspaces]);

  const handlePageChange = (newPage) => {
    setPageIndex(newPage);
  };

  const handlePageSizeChange = (newSize) => {
    setPageSize(newSize);
    setPageIndex(1);
  };

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
        setPageIndex(1);
        await fetchWorkspaces(1, pageSize);
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

  // Sort workspaces on current page
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
        totalCount={totalCount || workspaces.length}
        sortBy={sortBy}
        onSortChange={setSortBy}
        onRefresh={() => fetchWorkspaces(pageIndex, pageSize)}
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
          <button className="btn btn-secondary btn-sm" onClick={() => fetchWorkspaces(pageIndex, pageSize)}>
            Retry
          </button>
        </div>
      )}

      {/* 4. Workspace Grid or Empty State */}
      {!loading && (
        <>
          <WorkspaceGrid
            workspaces={sortedWorkspaces}
            onOpenCreateModal={() => setIsModalOpen(true)}
            onSelectWorkspace={(id) => navigate(`/workspace/${id}`)}
          />

          {/* Pagination */}
          {totalCount > 0 && (
            <Pagination
              currentPage={pageIndex}
              totalPages={totalPages}
              totalCount={totalCount}
              pageSize={pageSize}
              onPageChange={handlePageChange}
              onPageSizeChange={handlePageSizeChange}
              pageSizeOptions={[4, 8, 12, 24]}
              itemLabel="workspaces"
            />
          )}
        </>
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
