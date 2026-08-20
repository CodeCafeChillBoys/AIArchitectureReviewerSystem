import React from 'react';
import { FolderKanban, Plus } from 'lucide-react';

export default function EmptyWorkspaceState({ onOpenCreateModal }) {
  return (
    <div className="card" style={{ textAlign: 'center', padding: '56px 20px', backgroundColor: '#ffffff' }}>
      <div style={{
        width: '56px',
        height: '56px',
        borderRadius: '12px',
        backgroundColor: 'var(--accent-blue-light)',
        color: 'var(--accent-primary)',
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '16px',
      }}>
        <FolderKanban size={28} />
      </div>
      <h3 style={{ fontSize: '16px', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '6px' }}>
        No workspaces yet
      </h3>
      <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginBottom: '20px' }}>
        Create your first workspace to organize and review architecture system diagrams.
      </p>
      <button className="btn btn-primary" onClick={onOpenCreateModal}>
        <Plus size={16} />
        <span>Create First Workspace</span>
      </button>
    </div>
  );
}
