import React from 'react';
import { ArrowLeft, Plus, Upload, Layers } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function WorkspaceHeader({
  workspace,
  totalDiagrams = 0,
  onOpenMermaidModal,
  onOpenUploadModal,
}) {
  const navigate = useNavigate();

  return (
    <div style={{ marginBottom: '28px' }}>
      {/* Back button */}
      <button
        onClick={() => navigate('/dashboard')}
        className="btn btn-outline btn-sm"
        style={{
          display: 'inline-flex',
          alignItems: 'center',
          gap: '6px',
          marginBottom: '16px',
          fontSize: '13px',
          color: 'var(--text-secondary)',
        }}
      >
        <ArrowLeft size={15} />
        <span>Back to Dashboard</span>
      </button>

      {/* Main header row */}
      <div style={{
        display: 'flex',
        alignItems: 'flex-start',
        justifyContent: 'space-between',
        flexWrap: 'wrap',
        gap: '20px',
      }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            <h1 style={{ fontSize: '24px', fontWeight: 700, color: 'var(--text-primary)' }}>
              {workspace?.name || 'Workspace'}
            </h1>
            <span className="badge badge-active">Active</span>
          </div>

          <div style={{
            display: 'flex',
            alignItems: 'center',
            gap: '16px',
            marginTop: '8px',
            fontSize: '13px',
            color: 'var(--text-muted)',
          }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <Layers size={15} color="var(--accent-primary)" />
              <span>{totalDiagrams} {totalDiagrams === 1 ? 'Diagram' : 'Diagrams'}</span>
            </div>
            {workspace?.createdAt && (
              <span>• Created: {new Date(workspace.createdAt).toLocaleDateString('en-US')}</span>
            )}
          </div>
        </div>

        {/* Action buttons */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', flexWrap: 'wrap' }}>
          <button
            className="btn btn-outline"
            onClick={onOpenUploadModal}
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: '8px',
              height: '38px',
            }}
          >
            <Upload size={16} />
            <span>Upload Diagram Image</span>
          </button>

          <button
            className="btn btn-primary"
            onClick={onOpenMermaidModal}
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: '8px',
              height: '38px',
            }}
          >
            <Plus size={16} />
            <span>New Mermaid Diagram</span>
          </button>
        </div>
      </div>
    </div>
  );
}
