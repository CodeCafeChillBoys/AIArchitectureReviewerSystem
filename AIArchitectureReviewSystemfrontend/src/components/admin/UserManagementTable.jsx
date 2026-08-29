import React from 'react';
import { ShieldCheck, UserX, Trash2, RefreshCw } from 'lucide-react';

export default function UserManagementTable({
  users,
  loading,
  actionLoading,
  onToggleUserRole,
  onDeleteUser,
}) {
  return (
    <div className="card" style={{ padding: 0, overflow: 'hidden', border: '1px solid var(--border-color)' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '13px' }}>
        <thead>
          <tr style={{ backgroundColor: '#f8fafc', borderBottom: '1px solid var(--border-color)', color: 'var(--text-secondary)' }}>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Tài khoản / Email</th>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Vai trò (Role)</th>
            <th style={{ padding: '12px 16px', fontWeight: 600 }}>Ngày tạo</th>
            <th style={{ padding: '12px 16px', fontWeight: 600, textAlign: 'right' }}>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          {loading ? (
            <tr>
              <td colSpan={4} style={{ padding: '32px', textAlign: 'center', color: 'var(--text-muted)' }}>
                <RefreshCw size={20} className="animate-spin" style={{ margin: '0 auto 8px' }} />
                <div>Đang tải danh sách người dùng...</div>
              </td>
            </tr>
          ) : users.length === 0 ? (
            <tr>
              <td colSpan={4} style={{ padding: '32px', textAlign: 'center', color: 'var(--text-muted)' }}>
                Không tìm thấy người dùng nào.
              </td>
            </tr>
          ) : (
            users.map((u) => (
              <tr key={u.id} style={{ borderBottom: '1px solid #f1f5f9', transition: 'background-color 0.15s' }}>
                <td style={{ padding: '12px 16px', fontWeight: 600, color: 'var(--text-primary)' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <div style={{
                      width: '28px',
                      height: '28px',
                      borderRadius: '50%',
                      backgroundColor: u.role === 'Admin' ? '#f5f3ff' : '#eff6ff',
                      color: u.role === 'Admin' ? '#7c3aed' : '#2563eb',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: '12px',
                      fontWeight: 700,
                    }}>
                      {(u.email || 'U')[0].toUpperCase()}
                    </div>
                    <div>
                      <div>{u.email}</div>
                      {u.userName && u.userName !== u.email && (
                        <div style={{ fontSize: '11.5px', color: 'var(--text-muted)', fontWeight: 400 }}>{u.userName}</div>
                      )}
                    </div>
                  </div>
                </td>
                <td style={{ padding: '12px 16px' }}>
                  <span style={{
                    padding: '3px 10px',
                    borderRadius: '12px',
                    fontSize: '12px',
                    fontWeight: 600,
                    backgroundColor: u.role === 'Admin' ? '#f5f3ff' : '#f0fdf4',
                    color: u.role === 'Admin' ? '#6d28d9' : '#16a34a',
                    border: `1px solid ${u.role === 'Admin' ? '#ddd6fe' : '#bbf7d0'}`,
                  }}>
                    {u.role === 'Admin' ? '🛡️ Admin' : '👤 User'}
                  </span>
                </td>
                <td style={{ padding: '12px 16px', color: 'var(--text-secondary)' }}>
                  {u.createdAt ? new Date(u.createdAt).toLocaleDateString('vi-VN') : 'N/A'}
                </td>
                <td style={{ padding: '12px 16px', textAlign: 'right' }}>
                  <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '6px' }}>
                    <button
                      onClick={() => onToggleUserRole(u)}
                      disabled={actionLoading}
                      className="btn btn-secondary"
                      style={{ padding: '5px 10px', fontSize: '12px' }}
                      title={u.role === 'Admin' ? 'Hạ quyền xuống User' : 'Nâng quyền lên Admin'}
                    >
                      {u.role === 'Admin' ? <UserX size={13} style={{ marginRight: '4px' }} /> : <ShieldCheck size={13} style={{ marginRight: '4px' }} />}
                      <span>{u.role === 'Admin' ? 'Hạ xuống User' : 'Nâng lên Admin'}</span>
                    </button>
                    <button
                      onClick={() => onDeleteUser(u.id, u.email)}
                      disabled={actionLoading}
                      className="btn btn-secondary"
                      style={{ padding: '5px 8px', fontSize: '12px', color: '#dc2626' }}
                      title="Xóa người dùng"
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
