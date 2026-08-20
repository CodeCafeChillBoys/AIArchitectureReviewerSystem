import React from 'react';
import { Layers, Plus, Upload } from 'lucide-react';

export default function EmptyDiagramState({ onOpenMermaidModal, onOpenUploadModal }) {
  return (
    <div
      className="card"
      style={{
        textAlign: 'center',
        padding: '60px 24px',
        backgroundColor: '#ffffff',
        border: '2px dashed var(--border-color)',
        borderRadius: 'var(--radius-lg)',
      }}
    >
      <div style={{
        width: '64px',
        height: '64px',
        borderRadius: '16px',
        backgroundColor: 'var(--accent-blue-light)',
        color: 'var(--accent-primary)',
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '20px',
      }}>
        <Layers size={32} />
      </div>

      <h2 style={{ fontSize: '18px', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '8px' }}>
        No diagrams in this workspace yet
      </h2>
      <p style={{
        fontSize: '13.5px',
        color: 'var(--text-secondary)',
        maxWidth: '480px',
        margin: '0 auto 24px auto',
        lineHeight: 1.5,
      }}>
        Start designing your system architecture with Mermaid.js code or upload diagram images for automated AI evaluation and review.
      </p>

      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '14px', flexWrap: 'wrap' }}>
        <button
          className="btn btn-outline"
          onClick={onOpenUploadModal}
          style={{ padding: '10px 18px', gap: '8px', fontSize: '13.5px' }}
        >
          <Upload size={16} />
          <span>Upload Diagram Image</span>
        </button>

        <button
          className="btn btn-primary"
          onClick={onOpenMermaidModal}
          style={{ padding: '10px 18px', gap: '8px', fontSize: '13.5px' }}
        >
          <Plus size={16} />
          <span>Create First Mermaid Diagram</span>
        </button>
      </div>
    </div>
  );
}
