import React from 'react';
import { FileText, Edit3, Trash2, RefreshCw } from 'lucide-react';

export default function SystemRulesTable({
  rules,
  loading,
  onViewRule,
  onEditRule,
  onDeleteRule,
}) {
  return (
    <div className="card" style={{ padding: 0, overflow: 'hidden', border: '1px solid var(--border-color)' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '13px' }}>
        <thead>
          <tr style={{ backgroundColor: '#f8fafc', borderBottom: '1px solid var(--border-color)', color: 'var(--text-secondary)' }}>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Tên Quy tắc</th>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Loại sơ đồ</th>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Trạng thái</th>
            <th style={{ padding: '12px 16px', fontWeight: 600, textAlign: 'right' }}>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          {loading ? (
            <tr>
              <td colSpan={4} style={{ padding: '32px', textAlign: 'center', color: 'var(--text-muted)' }}>
                <RefreshCw size={20} className="animate-spin" style={{ margin: '0 auto 8px' }} />
                <div>Đang tải danh sách System Rules...</div>
              </td>
            </tr>
          ) : rules.length === 0 ? (
            <tr>
              <td colSpan={4} style={{ padding: '32px', textAlign: 'center', color: 'var(--text-muted)' }}>
                Không tìm thấy quy tắc kiến trúc nào phù hợp.
              </td>
            </tr>
          ) : (
            rules.map((r) => (
              <tr key={r.id} style={{ borderBottom: '1px solid #f1f5f9', transition: 'background-color 0.15s' }}>
                <td style={{ padding: '12px 16px', fontWeight: 600, color: 'var(--text-primary)' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <FileText size={16} style={{ color: 'var(--accent-primary)', flexShrink: 0 }} />
                    <span>{r.ruleName}</span>
                  </div>
                </td>
                <td style={{ padding: '12px 16px' }}>
                  <span style={{
                    padding: '3px 8px',
                    borderRadius: '6px',
                    fontSize: '12px',
                    fontWeight: 500,
                    backgroundColor: '#eff6ff',
                    color: '#1d4ed8',
                    border: '1px solid #dbeafe',
                  }}>
                    {r.diagramType || 'Global'}
                  </span>
                </td>
                <td style={{ padding: '12px 16px' }}>
                  <span style={{
                    padding: '2px 8px',
                    borderRadius: '12px',
                    fontSize: '11.5px',
                    fontWeight: 600,
                    backgroundColor: r.isActive !== false ? '#ecfdf5' : '#f1f5f9',
                    color: r.isActive !== false ? '#059669' : '#64748b',
                  }}>
                    {r.isActive !== false ? 'Hoạt động' : 'Tắt'}
                  </span>
                </td>
                <td style={{ padding: '12px 16px', textAlign: 'right' }}>
                  <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '6px' }}>
                    <button
                      onClick={() => onViewRule(r)}
                      className="btn btn-secondary"
                      style={{ padding: '5px 8px', fontSize: '12px' }}
                      title="Xem chi tiết nội dung"
                    >
                      Xem
                    </button>
                    <button
                      onClick={() => onEditRule(r)}
                      className="btn btn-secondary"
                      style={{ padding: '5px 8px', fontSize: '12px' }}
                      title="Chỉnh sửa quy tắc"
                    >
                      <Edit3 size={13} />
                    </button>
                    <button
                      onClick={() => onDeleteRule(r.id, r.ruleName)}
                      className="btn btn-secondary"
                      style={{ padding: '5px 8px', fontSize: '12px', color: '#dc2626' }}
                      title="Xóa quy tắc"
                    >
                      <Trash2 size={13} />
                    </button>
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
