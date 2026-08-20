import React from 'react';
import { FolderKanban, Layers, Clock, ArrowRight, Sparkles } from 'lucide-react';

export default function WorkspaceCard({ workspace, onClick }) {
  const diagramCount = workspace.diagramCount ?? workspace.diagrams?.length ?? 0;
  const timeText = workspace.updatedAt || (workspace.createdAt ? new Date(workspace.createdAt).toLocaleDateString('en-US') : 'Just created');

  return (
    <div
      className="card"
      onClick={onClick}
      style={{
        cursor: 'pointer',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        padding: '20px',
        backgroundColor: '#ffffff',
        border: '1px solid var(--border-color)',
        borderRadius: 'var(--radius-lg)',
        transition: 'all 0.2s ease',
        position: 'relative',
        overflow: 'hidden',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = 'translateY(-2px)';
        e.currentTarget.style.boxShadow = '0 8px 20px rgba(0, 0, 0, 0.06)';
        e.currentTarget.style.borderColor = 'var(--accent-primary)';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = 'translateY(0)';
        e.currentTarget.style.boxShadow = 'none';
        e.currentTarget.style.borderColor = 'var(--border-color)';
      }}
    >
      <div>
        {/* Header: Icon + Status Badge */}
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          marginBottom: '14px',
        }}>
          <div style={{
            width: '40px',
            height: '40px',
            borderRadius: '10px',
            backgroundColor: 'var(--accent-blue-light)',
            color: 'var(--accent-primary)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <FolderKanban size={20} />
          </div>
          <span className={`badge ${workspace.status === 'Consistent' ? 'badge-success' : 'badge-warning'}`}>
            {workspace.status || 'Active'}
          </span>
        </div>

        {/* Workspace Name */}
        <h3 style={{
          fontSize: '16px',
          fontWeight: 600,
          color: 'var(--text-primary)',
          marginBottom: '6px',
          lineHeight: '1.3',
        }}>
          {workspace.name}
        </h3>

        {/* Description */}
        <p style={{
          fontSize: '13px',
          color: 'var(--text-secondary)',
          lineHeight: '1.4',
          marginBottom: '14px',
          display: '-webkit-box',
          WebkitLineClamp: 2,
          WebkitBoxOrient: 'vertical',
          overflow: 'hidden',
          minHeight: '36px',
        }}>
          {workspace.description || 'Workspace for system architectural diagrams and consistency review.'}
        </p>

        {/* AI Ready Tag */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '6px', marginBottom: '4px' }}>
          <span style={{
            fontSize: '11.5px',
            fontWeight: 500,
            display: 'inline-flex',
            alignItems: 'center',
            gap: '4px',
            padding: '2px 8px',
            borderRadius: '12px',
            backgroundColor: '#f0fdf4',
            color: '#16a34a',
            border: '1px solid #bbf7d0',
          }}>
            <Sparkles size={11} />
            <span>AI Review Ready</span>
          </span>
        </div>
      </div>

      {/* Footer Info */}
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        fontSize: '12.5px',
        color: 'var(--text-secondary)',
        marginTop: '16px',
        paddingTop: '12px',
        borderTop: '1px solid var(--border-color)',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
          <span style={{ display: 'flex', alignItems: 'center', gap: '5px' }}>
            <Layers size={14} color="var(--accent-primary)" />
            {diagramCount} {diagramCount === 1 ? 'Diagram' : 'Diagrams'}
          </span>
          <span style={{ display: 'flex', alignItems: 'center', gap: '5px', color: 'var(--text-muted)' }}>
            <Clock size={13} />
            {timeText}
          </span>
        </div>

        <div style={{
          display: 'flex',
          alignItems: 'center',
          gap: '4px',
          fontSize: '12px',
          fontWeight: 600,
          color: 'var(--accent-primary)',
        }}>
          <span>Open</span>
          <ArrowRight size={13} />
        </div>
      </div>
    </div>
  );
}
