import React from 'react';
import { Search, Bell, User } from 'lucide-react';

export default function Header({
  title = 'Dashboard',
  subtitle = 'Overview of active architectural workspaces and recent AI reviews.',
  actions
}) {
  return (
    <header className="header">
      {/* Khối bên Trái: Title & Subtitle */}
      <div>
        <h1 className="header-title">{title}</h1>
        {subtitle && (
          <p style={{ fontSize: '12px', color: 'var(--text-muted)', marginTop: '2px' }}>
            {subtitle}
          </p>
        )}
      </div>

      {/* Khối bên Phải: Search bar, Actions, Chuông thông báo, User */}
      <div className="header-actions">
        {/* 1. Ô Tìm kiếm nhanh */}
        <div style={{ position: 'relative', width: '260px' }}>
          <Search
            size={16}
            style={{
              position: 'absolute',
              left: '12px',
              top: '50%',
              transform: 'translateY(-50%)',
              color: 'var(--text-muted)'
            }}
          />
          <input
            type="text"
            placeholder="Search workspaces, diagrams..."
            className="input-text"
            style={{ paddingLeft: '36px', height: '36px', fontSize: '12.5px' }}
          />
        </div>

        {/* 2. Các nút hành động riêng của từng màn hình (nếu có) */}
        {actions}

        {/* 3. Nút chuông thông báo */}
        <button className="btn btn-outline btn-sm" style={{ padding: '8px', borderRadius: '50%' }}>
          <Bell size={16} />
        </button>

        {/* 4. Avatar người dùng */}
        <div style={{
          display: 'flex',
          alignItems: 'center',
          gap: '8px',
          padding: '4px 10px',
          background: 'var(--bg-card)',
          borderRadius: '20px',
          border: '1px solid var(--border-color)',
          fontSize: '12px',
          color: 'var(--text-secondary)'
        }}>
          <div style={{
            width: '24px',
            height: '24px',
            borderRadius: '50%',
            background: '#2563eb',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: '#fff'
          }}>
            <User size={13} />
          </div>
          <span>{(() => {
            try {
              const u = JSON.parse(localStorage.getItem('user'));
              return u?.fullName || u?.email || 'User';
            } catch {
              return 'User';
            }
          })()}</span>
        </div>
      </div>
    </header>
  );
}

