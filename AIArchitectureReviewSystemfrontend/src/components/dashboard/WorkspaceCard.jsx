import React from 'react';
import { FolderKanban, Layers, Clock } from 'lucide-react';

export default function WorkspaceCard({ workspace, onClick }) {
  const diagramCount = workspace.diagramCount ?? workspace.diagrams?.length ?? 0;
  const timeText = workspace.updatedAt || (workspace.createdAt ? new Date(workspace.createdAt).toLocaleDateString() : 'Mới cập nhật');

  return (
    <div
      className="card"
      onClick={onClick}
      style={{
        cursor: 'pointer',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
      }}
    >
      <div>
        {/* Header của Card: Icon + Status Badge */}
        <div style={{
          display: 'flex',
          alignItems: 'flex-start',
          justifyContent: 'space-between',
          marginBottom: '14px',
        }}>
          <div style={{
            width: '38px',
            height: '38px',
            borderRadius: 'var(--radius-sm)',
            backgroundColor: 'var(--accent-blue-light)',
            color: 'var(--accent-primary)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <FolderKanban size={18} />
          </div>
          <span className={`badge ${workspace.status === 'Consistent' ? 'badge-success' : 'badge-warning'}`}>
            {workspace.status || 'Active'}
          </span>
        </div>

        {/* Tên Workspace */}
        <h3 style={{
          fontSize: '15.5px',
          fontWeight: 600,
          color: 'var(--text-primary)',
          marginBottom: '6px',
          lineHeight: '1.3',
        }}>
          {workspace.name}
        </h3>
      </div>

      {/* Thông tin Meta: Số lượng diagram + Thời gian */}
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        fontSize: '12.5px',
        color: 'var(--text-secondary)',
        marginTop: '20px',
        paddingTop: '12px',
        borderTop: '1px solid var(--border-color)',
      }}>
        <span style={{ display: 'flex', alignItems: 'center', gap: '5px' }}>
          <Layers size={14} color="var(--accent-primary)" />
          {diagramCount} Diagrams
        </span>
        <span style={{ display: 'flex', alignItems: 'center', gap: '5px' }}>
          <Clock size={13} />
          {timeText}
        </span>
      </div>
    </div>
  );
}
