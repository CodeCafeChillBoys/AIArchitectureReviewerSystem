import React from 'react';
import { Search, Plus, Upload, Database, Trash2 } from 'lucide-react';

export default function AdminActionBar({
  activeTab,
  searchTerm,
  onSearchChange,
  typeFilter,
  onTypeFilterChange,
  diagramTypes,
  onOpenRuleModal,
  onOpenUploadModal,
  onSeedRules,
  onClearAllRules,
  onOpenPromptModal,
  actionLoading,
}) {
  return (
    <div style={{
      display: 'flex',
      flexWrap: 'wrap',
      justifyContent: 'space-between',
      alignItems: 'center',
      gap: '12px',
      marginBottom: '16px',
    }}>
      {/* Search & Filter */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '10px', flex: '1 1 300px', maxWidth: '500px' }}>
        <div style={{ position: 'relative', width: '100%' }}>
          <Search size={15} style={{ position: 'absolute', left: '10px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
          <input
            type="text"
            className="input-text"
            placeholder={
              activeTab === 'rules'
                ? 'Tìm kiếm quy tắc theo tên...'
                : activeTab === 'prompts'
                ? 'Tìm kiếm prompt template...'
                : 'Tìm kiếm người dùng theo email...'
            }
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
            style={{ paddingLeft: '32px', fontSize: '13px' }}
          />
        </div>

        {activeTab !== 'users' ? (
          <select
            className="input-text"
            value={typeFilter}
            onChange={(e) => onTypeFilterChange(e.target.value)}
            style={{ width: '180px', fontSize: '13px' }}
          >
            <option value="ALL">Tất cả loại sơ đồ</option>
            {diagramTypes.map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
        ) : (
          <select
            className="input-text"
            value={typeFilter}
            onChange={(e) => onTypeFilterChange(e.target.value)}
            style={{ width: '140px', fontSize: '13px' }}
          >
            <option value="ALL">Tất cả vai trò</option>
            <option value="Admin">Admin</option>
            <option value="User">User</option>
          </select>
        )}
      </div>

      {/* Action Buttons */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        {activeTab === 'rules' && (
          <>
            <button
              onClick={() => onOpenRuleModal()}
              className="btn btn-primary"
              style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px' }}
            >
              <Plus size={16} />
              <span>Thêm Quy tắc</span>
            </button>
            <button
              onClick={onOpenUploadModal}
              className="btn btn-secondary"
              style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px' }}
            >
              <Upload size={15} />
              <span>Upload File</span>
            </button>
            <button
              onClick={onSeedRules}
              disabled={actionLoading}
              className="btn btn-secondary"
              title="Tự động đồng bộ các file trong thư mục RAG_Documents"
              style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px' }}
            >
              <Database size={15} />
              <span>Seed RAG Docs</span>
            </button>
            <button
              onClick={onClearAllRules}
              disabled={actionLoading}
              className="btn"
              title="Xóa toàn bộ quy tắc trong DB"
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '6px',
                fontSize: '13px',
                backgroundColor: '#fee2e2',
                color: '#dc2626',
                border: '1px solid #fecaca',
              }}
            >
              <Trash2 size={15} />
              <span>Xóa hết</span>
            </button>
          </>
        )}

        {activeTab === 'prompts' && (
          <button
            onClick={() => onOpenPromptModal()}
            className="btn btn-primary"
            style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px' }}
          >
            <Plus size={16} />
            <span>Tạo Prompt Mới</span>
          </button>
        )}
      </div>
    </div>
  );
}
