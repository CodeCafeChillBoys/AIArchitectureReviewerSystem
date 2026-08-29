import React from 'react';
import { X } from 'lucide-react';

export default function ViewItemModal({ isOpen, onClose, item }) {
  if (!isOpen || !item) return null;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.45)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 1000,
      padding: '20px',
    }}>
      <div style={{
        backgroundColor: '#ffffff',
        borderRadius: '12px',
        width: '100%',
        maxWidth: '750px',
        maxHeight: '85vh',
        display: 'flex',
        flexDirection: 'column',
        boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)',
        padding: '24px',
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
          <div>
            <span style={{ fontSize: '12px', fontWeight: 600, color: 'var(--accent-primary)', textTransform: 'uppercase' }}>
              {item.type}
            </span>
            <h3 style={{ fontSize: '18px', fontWeight: 700, margin: '2px 0 0', color: 'var(--text-primary)' }}>
              {item.title}
            </h3>
          </div>
          <button
            onClick={onClose}
            style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)' }}
          >
            <X size={18} />
          </button>
        </div>

        <div style={{
          flex: 1,
          overflowY: 'auto',
          backgroundColor: '#f8fafc',
          border: '1px solid var(--border-color)',
          borderRadius: '8px',
          padding: '16px',
          fontFamily: 'monospace',
          fontSize: '12.5px',
          whiteSpace: 'pre-wrap',
          wordBreak: 'break-word',
          color: '#334155',
        }}>
          {item.content}
        </div>

        <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '16px' }}>
          <button
            onClick={onClose}
            className="btn btn-secondary"
          >
            Đóng
          </button>
        </div>
      </div>
    </div>
  );
}
