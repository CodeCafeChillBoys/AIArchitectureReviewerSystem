import React from 'react';
import { Plus, RefreshCw, FolderKanban, ArrowUpDown, Filter } from 'lucide-react';

export default function DashboardHeader({
  totalCount = 0,
  sortBy = 'newest',
  onSortChange,
  onRefresh,
  loading = false,
  onOpenCreateModal,
}) {
  return (
    <div style={{
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      flexWrap: 'wrap',
      gap: '16px',
      marginBottom: '24px',
      paddingBottom: '16px',
      borderBottom: '1px solid var(--border-color)',
    }}>
      {/* Khối bên Trái: Thống kê & Tabs */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
        <div style={{
          display: 'flex',
          alignItems: 'center',
          gap: '8px',
          padding: '6px 12px',
          backgroundColor: '#ffffff',
          borderRadius: 'var(--radius-md)',
          border: '1px solid var(--border-color)',
          fontSize: '13px',
          fontWeight: 600,
          color: 'var(--text-primary)',
        }}>
          <FolderKanban size={16} color="var(--accent-primary)" />
          <span>Workspaces</span>
          <span style={{
            fontSize: '11px',
            backgroundColor: 'var(--accent-blue-light)',
            color: 'var(--accent-primary)',
            padding: '1px 7px',
            borderRadius: '10px',
            fontWeight: 700,
          }}>
            {totalCount}
          </span>
        </div>
      </div>

      {/* Khối bên Phải: Sort, Refresh, Nút New Workspace */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '10px', flexWrap: 'wrap' }}>
        {/* Bộ lọc sắp xếp */}
        <div style={{
          display: 'flex',
          alignItems: 'center',
          gap: '6px',
          backgroundColor: '#ffffff',
          border: '1px solid var(--border-color)',
          borderRadius: 'var(--radius-md)',
          padding: '0 10px',
          height: '36px',
        }}>
          <ArrowUpDown size={14} color="var(--text-muted)" />
          <select
            value={sortBy}
            onChange={(e) => onSortChange?.(e.target.value)}
            style={{
              border: 'none',
              outline: 'none',
              background: 'transparent',
              fontSize: '12.5px',
              color: 'var(--text-secondary)',
              cursor: 'pointer',
              fontWeight: 500,
            }}
          >
            <option value="newest">Newest</option>
            <option value="oldest">Oldest</option>
            <option value="name_asc">Name: A → Z</option>
            <option value="name_desc">Name: Z → A</option>
          </select>
        </div>

        {/* Refresh button */}
        {onRefresh && (
          <button
            className="btn btn-outline"
            onClick={onRefresh}
            disabled={loading}
            title="Refresh workspaces"
            style={{
              height: '36px',
              width: '36px',
              padding: 0,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            <RefreshCw size={14} className={loading ? 'animate-spin' : ''} />
          </button>
        )}

        {/* Nút Tạo Workspace Mới */}
        <button
          className="btn btn-primary"
          onClick={onOpenCreateModal}
          style={{
            height: '36px',
            display: 'inline-flex',
            alignItems: 'center',
            gap: '6px',
            fontWeight: 500,
          }}
        >
          <Plus size={16} />
          <span>New Workspace</span>
        </button>
      </div>
    </div>
  );
}
