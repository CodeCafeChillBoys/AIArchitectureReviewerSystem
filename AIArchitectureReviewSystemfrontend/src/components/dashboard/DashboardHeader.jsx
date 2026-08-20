import React from 'react';
import { Search, Plus } from 'lucide-react';

export default function DashboardHeader({ searchTerm, onSearchChange, onOpenCreateModal }) {
  return (
    <div style={{
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      flexWrap: 'wrap',
      gap: '16px',
      marginBottom: '28px',
    }}>
      <div>
        <h1 style={{ fontSize: '22px', fontWeight: 700, color: 'var(--text-primary)' }}>
          Workspaces
        </h1>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>
          Tổng hợp các không gian thiết kế kiến trúc và sơ đồ hệ thống của bạn.
        </p>
      </div>

      <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
        {/* Ô Tìm kiếm nhanh */}
        <div style={{ position: 'relative', width: '240px' }}>
          <Search
            size={15}
            style={{
              position: 'absolute',
              left: '10px',
              top: '50%',
              transform: 'translateY(-50%)',
              color: 'var(--text-muted)'
            }}
          />
          <input
            type="text"
            className="input-text"
            placeholder="Tìm workspace..."
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
            style={{ paddingLeft: '32px', height: '36px', fontSize: '13px' }}
          />
        </div>

        {/* Nút Tạo Workspace Mới */}
        <button
          className="btn btn-primary"
          onClick={onOpenCreateModal}
          style={{ height: '36px' }}
        >
          <Plus size={16} />
          <span>New Workspace</span>
        </button>
      </div>
    </div>
  );
}
