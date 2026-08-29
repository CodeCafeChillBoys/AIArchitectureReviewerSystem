import React from 'react';
import { X, Upload } from 'lucide-react';

export default function RuleUploadModal({
  isOpen,
  onClose,
  uploadDiagramType,
  onDiagramTypeChange,
  uploadFile,
  onFileChange,
  onSubmit,
  actionLoading,
  diagramTypes,
}) {
  if (!isOpen) return null;

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
        maxWidth: '500px',
        boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)',
        padding: '24px',
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
          <h3 style={{ fontSize: '17px', fontWeight: 700, margin: 0, color: 'var(--text-primary)' }}>
            Tải lên File Quy tắc Kiến trúc
          </h3>
          <button
            onClick={onClose}
            style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)' }}
          >
            <X size={18} />
          </button>
        </div>

        <form onSubmit={onSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
          <div>
            <label style={{ display: 'block', fontSize: '12.5px', fontWeight: 600, color: 'var(--text-secondary)', marginBottom: '4px' }}>
              Loại sơ đồ áp dụng
            </label>
            <select
              className="input-text"
              value={uploadDiagramType}
              onChange={(e) => onDiagramTypeChange(e.target.value)}
            >
              {diagramTypes.map((t) => (
                <option key={t} value={t}>{t}</option>
              ))}
            </select>
          </div>

          <div>
            <label style={{ display: 'block', fontSize: '12.5px', fontWeight: 600, color: 'var(--text-secondary)', marginBottom: '4px' }}>
              Chọn File Markdown (.md) hoặc Text (.txt) *
            </label>
            <input
              type="file"
              accept=".md,.txt"
              className="input-text"
              onChange={(e) => onFileChange(e.target.files?.[0] || null)}
              required
            />
          </div>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '10px' }}>
            <button
              type="button"
              onClick={onClose}
              className="btn btn-secondary"
              disabled={actionLoading}
            >
              Hủy
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={actionLoading || !uploadFile}
              style={{ display: 'flex', alignItems: 'center', gap: '6px' }}
            >
              <Upload size={15} />
              <span>{actionLoading ? 'Đang tải lên...' : 'Tải lên'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
