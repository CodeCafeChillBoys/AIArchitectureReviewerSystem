import React from 'react';
import {
  FileCode,
  Image as ImageIcon,
  GitBranch,
  Clock,
  ExternalLink,
  Bot,
  Layers,
  ArrowRight,
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';

const STATUS_CONFIG = {
  0: { label: 'Pending', color: 'var(--color-warning)', bg: 'var(--color-warning-bg)', border: '#fde68a' },
  1: { label: 'Processing', color: 'var(--accent-primary)', bg: 'var(--accent-blue-light)', border: '#bfdbfe' },
  2: { label: 'Completed', color: 'var(--color-success)', bg: 'var(--color-success-bg)', border: '#bbf7d0' },
  3: { label: 'Failed', color: 'var(--color-danger)', bg: 'var(--color-danger-bg)', border: '#fecaca' },
};

export default function DiagramCard({ diagram }) {
  const navigate = useNavigate();

  const status = STATUS_CONFIG[diagram.currentStatus] || {
    label: 'Pending',
    color: 'var(--text-secondary)',
    bg: '#f1f5f9',
    border: '#e2e8f0',
  };

  const isMermaid = diagram.currentStorageUrl?.endsWith('.mmd') || diagram.rawFormat === 'mermaid';

  return (
    <div
      className="card"
      style={{
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        padding: '20px',
        borderRadius: 'var(--radius-lg)',
        border: '1px solid var(--border-color)',
        transition: 'all 0.2s ease',
        cursor: 'pointer',
        backgroundColor: '#ffffff',
      }}
      onClick={() => navigate(`/editor/${diagram.id}`)}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = 'translateY(-2px)';
        e.currentTarget.style.boxShadow = '0 6px 16px rgba(0, 0, 0, 0.06)';
        e.currentTarget.style.borderColor = 'var(--accent-primary)';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = 'translateY(0)';
        e.currentTarget.style.boxShadow = 'none';
        e.currentTarget.style.borderColor = 'var(--border-color)';
      }}
    >
      <div>
        {/* Header row: Type icon, Badge status */}
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '14px' }}>
          <div style={{
            width: '40px',
            height: '40px',
            borderRadius: '10px',
            backgroundColor: isMermaid ? 'var(--accent-blue-light)' : '#f3e8ff',
            color: isMermaid ? 'var(--accent-primary)' : '#7e22ce',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            {isMermaid ? <FileCode size={20} /> : <ImageIcon size={20} />}
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <span
              style={{
                fontSize: '11px',
                fontWeight: 600,
                padding: '3px 8px',
                borderRadius: '12px',
                color: status.color,
                backgroundColor: status.bg,
                border: `1px solid ${status.border}`,
              }}
            >
              {status.label}
            </span>
            <span
              style={{
                fontSize: '11px',
                fontWeight: 600,
                padding: '3px 8px',
                borderRadius: '12px',
                color: 'var(--text-secondary)',
                backgroundColor: '#f1f5f9',
              }}
            >
              v{diagram.currentVersion || 1}
            </span>
          </div>
        </div>

        {/* Title */}
        <h3 style={{
          fontSize: '16px',
          fontWeight: 600,
          color: 'var(--text-primary)',
          marginBottom: '6px',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          whiteSpace: 'nowrap',
        }}>
          {diagram.name}
        </h3>

        {/* Diagram Type tag */}
        <div style={{ display: 'inline-block', marginBottom: '10px' }}>
          <span style={{
            fontSize: '12px',
            fontWeight: 500,
            color: 'var(--accent-primary)',
            backgroundColor: 'var(--accent-blue-light)',
            padding: '2px 8px',
            borderRadius: '4px',
          }}>
            {diagram.diagramType || 'Diagram'}
          </span>
        </div>

        {/* Description */}
        <p style={{
          fontSize: '13px',
          color: 'var(--text-secondary)',
          lineHeight: '1.4',
          marginBottom: '16px',
          display: '-webkit-box',
          WebkitLineClamp: 2,
          WebkitBoxOrient: 'vertical',
          overflow: 'hidden',
          minHeight: '36px',
        }}>
          {diagram.description || 'No description provided for this diagram.'}
        </p>
      </div>

      {/* Footer Info & Actions */}
      <div style={{
        paddingTop: '12px',
        borderTop: '1px solid var(--border-color)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
      }}>
        <div style={{
          display: 'flex',
          alignItems: 'center',
          gap: '6px',
          fontSize: '12px',
          color: 'var(--text-muted)',
        }}>
          <Clock size={13} />
          <span>
            {diagram.createdAt ? new Date(diagram.createdAt).toLocaleDateString('en-US') : 'Just created'}
          </span>
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <button
            className="btn btn-primary btn-sm"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`/review/${diagram.id}`);
            }}
            title="View AI Architecture Review Report"
            style={{ padding: '6px 16px', fontSize: '13px', whiteSpace: 'nowrap' }}
          >
            View Report
          </button>
        </div>
      </div>
    </div>
  );
}
