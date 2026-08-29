import React from 'react';
import { Shield, RefreshCw } from 'lucide-react';

export default function AdminHeader({ onRefresh, loading, actionLoading }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '24px' }}>
      <div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '4px' }}>
          <div style={{
            width: '32px',
            height: '32px',
            borderRadius: '8px',
            backgroundColor: '#e0e7ff',
            color: '#4338ca',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <Shield size={18} />
          </div>
          <h1 style={{ fontSize: '22px', fontWeight: 700, color: 'var(--text-primary)', margin: 0 }}>
            Admin Console
          </h1>
          <span style={{
            fontSize: '11px',
            fontWeight: 600,
            padding: '2px 8px',
            borderRadius: '12px',
            backgroundColor: '#f1f5f9',
            color: '#475569',
          }}>
            Management Hub
          </span>
        </div>
        <p style={{ fontSize: '13px', color: 'var(--text-secondary)', margin: 0 }}>
          Quản lý tập trung các quy tắc kiến trúc RAG, mẫu System Prompts và danh sách người dùng hệ thống.
        </p>
      </div>

      <button
        onClick={onRefresh}
        disabled={loading || actionLoading}
        className="btn btn-secondary"
        style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px' }}
      >
        <RefreshCw size={15} className={loading ? 'animate-spin' : ''} />
        <span>Làm mới</span>
      </button>
    </div>
  );
}
