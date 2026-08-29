import React from 'react';
import { X, Save } from 'lucide-react';

export default function RuleFormModal({
  isOpen,
  onClose,
  editingRule,
  formData,
  onFormChange,
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
        maxWidth: '650px',
        maxHeight: '90vh',
        overflowY: 'auto',
        boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)',
        padding: '24px',
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
          <h3 style={{ fontSize: '17px', fontWeight: 700, margin: 0, color: 'var(--text-primary)' }}>
            {editingRule ? 'Chỉnh sửa Quy tắc Kiến trúc' : 'Thêm mới Quy tắc Kiến trúc'}
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
              Tên Quy tắc / File Name *
            </label>
            <input
              type="text"
              className="input-text"
              placeholder="Ví dụ: SequenceDiagramRules, DatabaseAccessGuideline"
              value={formData.ruleName}
              onChange={(e) => onFormChange({ ...formData, ruleName: e.target.value })}
              required
            />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <div>
              <label style={{ display: 'block', fontSize: '12.5px', fontWeight: 600, color: 'var(--text-secondary)', marginBottom: '4px' }}>
                Loại sơ đồ
              </label>
              <select
                className="input-text"
                value={formData.diagramType}
                onChange={(e) => onFormChange({ ...formData, diagramType: e.target.value })}
              >
                {diagramTypes.map((t) => (
                  <option key={t} value={t}>{t}</option>
                ))}
              </select>
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '8px', paddingTop: '22px' }}>
              <input
                type="checkbox"
                id="isActiveCheckModal"
                checked={formData.isActive}
                onChange={(e) => onFormChange({ ...formData, isActive: e.target.checked })}
                style={{ width: '16px', height: '16px', cursor: 'pointer' }}
              />
              <label htmlFor="isActiveCheckModal" style={{ fontSize: '13px', fontWeight: 500, cursor: 'pointer' }}>
                Kích hoạt áp dụng vào RAG
              </label>
            </div>
          </div>

          <div>
            <label style={{ display: 'block', fontSize: '12.5px', fontWeight: 600, color: 'var(--text-secondary)', marginBottom: '4px' }}>
              Nội dung quy tắc / Markdown Guideline
            </label>
            <textarea
              className="input-text"
              rows={10}
              placeholder="Mô tả chi tiết các nguyên tắc kiến trúc, dấu hiệu vi phạm và khuyến nghị sửa chữa..."
              value={formData.regexOrCondition}
              onChange={(e) => onFormChange({ ...formData, regexOrCondition: e.target.value })}
              style={{ fontFamily: 'monospace', fontSize: '12.5px', resize: 'vertical' }}
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
              disabled={actionLoading}
              style={{ display: 'flex', alignItems: 'center', gap: '6px' }}
            >
              <Save size={15} />
              <span>{actionLoading ? 'Đang lưu...' : 'Lưu Quy tắc'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
