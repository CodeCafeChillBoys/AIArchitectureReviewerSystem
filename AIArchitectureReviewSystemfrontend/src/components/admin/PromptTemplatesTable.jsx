import React from 'react';
import { Sparkles, Edit3, Trash2, RefreshCw } from 'lucide-react';

export default function PromptTemplatesTable({
  prompts,
  loading,
  onViewPrompt,
  onEditPrompt,
  onDeletePrompt,
}) {
  return (
    <div className="card" style={{ padding: 0, overflow: 'hidden', border: '1px solid var(--border-color)' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '13px' }}>
        <thead>
          <tr style={{ backgroundColor: '#f8fafc', borderBottom: '1px solid var(--border-color)', color: 'var(--text-secondary)' }}>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Tên Prompt Template</th>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Loại sơ đồ áp dụng</th>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Xem trước nội dung</th>
            <th style={{ padding: '12px 16px', fontWeight: 600, textAlign: 'right' }}>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          {loading ? (
            <tr>
              <td colSpan={4} style={{ padding: '32px', textAlign: 'center', color: 'var(--text-muted)' }}>
                <RefreshCw size={20} className="animate-spin" style={{ margin: '0 auto 8px' }} />
                <div>Đang tải danh sách Prompt Templates...</div>
              </td>
            </tr>
          ) : prompts.length === 0 ? (
            <tr>
              <td colSpan={4} style={{ padding: '32px', textAlign: 'center', color: 'var(--text-muted)' }}>
                Chưa có mẫu prompt nào. Hãy bấm "Tạo Prompt Mới" để thêm.
              </td>
            </tr>
          ) : (
            prompts.map((p) => (
              <tr key={p.id} style={{ borderBottom: '1px solid #f1f5f9', transition: 'background-color 0.15s' }}>
                <td style={{ padding: '12px 16px', fontWeight: 600, color: 'var(--text-primary)' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Sparkles size={16} style={{ color: '#7c3aed', flexShrink: 0 }} />
                    <span>{p.name}</span>
                  </div>
                </td>
                <td style={{ padding: '12px 16px' }}>
                  <span style={{
                    padding: '3px 8px',
                    borderRadius: '6px',
                    fontSize: '12px',
                    fontWeight: 500,
                    backgroundColor: '#f5f3ff',
                    color: '#6d28d9',
                    border: '1px solid #ede9fe',
                  }}>
                    {p.diagramType || 'Global'}
                  </span>
                </td>
                <td style={{ padding: '12px 16px', color: 'var(--text-secondary)', maxWidth: '380px' }}>
                  <div style={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {p.content}
                  </div>
                </td>
                <td style={{ padding: '12px 16px', textAlign: 'right' }}>
                  <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '6px' }}>
                    <button
                      onClick={() => onViewPrompt(p)}
                      className="btn btn-secondary"
                      style={{ padding: '5px 8px', fontSize: '12px' }}
                      title="Xem toàn bộ prompt"
                    >
                      Xem
                    </button>
                    <button
                      onClick={() => onEditPrompt(p)}
                      className="btn btn-secondary"
                      style={{ padding: '5px 8px', fontSize: '12px' }}
                      title="Sửa prompt"
                    >
                      <Edit3 size={13} />
                    </button>
                    <button
                      onClick={() => onDeletePrompt(p.id, p.name)}
                      className="btn btn-secondary"
                      style={{ padding: '5px 8px', fontSize: '12px', color: '#dc2626' }}
                      title="Xóa prompt"
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
