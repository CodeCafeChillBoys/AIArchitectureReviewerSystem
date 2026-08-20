import React, { useState, useRef, useEffect } from 'react';
import { Search, Bell, User, LogOut, Settings, ChevronDown } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/authService';

export default function Header({
  title = 'Dashboard',
  subtitle = 'Overview of active architectural workspaces and recent AI reviews.',
  actions,
}) {
  const navigate = useNavigate();
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const dropdownRef = useRef(null);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const getUserData = () => {
    try {
      const u = JSON.parse(localStorage.getItem('user'));
      return {
        fullName: u?.fullName || u?.email || 'User',
        email: u?.email || '',
      };
    } catch {
      return { fullName: 'User', email: '' };
    }
  };

  const userData = getUserData();

  const handleLogout = () => {
    authService.logout();
  };

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

      {/* Khối bên Phải: Search bar, Actions, Chuông thông báo, User Dropdown */}
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
              color: 'var(--text-muted)',
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

        {/* 4. Avatar & Dropdown Người dùng */}
        <div style={{ position: 'relative' }} ref={dropdownRef}>
          <button
            onClick={() => setDropdownOpen((prev) => !prev)}
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              padding: '5px 12px',
              background: dropdownOpen ? 'var(--accent-blue-light)' : 'var(--bg-card)',
              borderRadius: '20px',
              border: `1px solid ${dropdownOpen ? 'var(--accent-primary)' : 'var(--border-color)'}`,
              fontSize: '12.5px',
              color: 'var(--text-primary)',
              cursor: 'pointer',
              transition: 'all 0.15s ease',
              fontWeight: 500,
            }}
          >
            <div style={{
              width: '24px',
              height: '24px',
              borderRadius: '50%',
              background: '#2563eb',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: '#fff',
              flexShrink: 0,
            }}>
              <User size={13} />
            </div>
            <span>{userData.fullName}</span>
            <ChevronDown size={14} color="var(--text-muted)" style={{
              transform: dropdownOpen ? 'rotate(180deg)' : 'rotate(0)',
              transition: 'transform 0.2s ease',
            }} />
          </button>

          {/* Dropdown Menu */}
          {dropdownOpen && (
            <div style={{
              position: 'absolute',
              top: 'calc(100% + 8px)',
              right: 0,
              width: '220px',
              backgroundColor: '#ffffff',
              borderRadius: 'var(--radius-md)',
              border: '1px solid var(--border-color)',
              boxShadow: '0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1)',
              padding: '6px',
              zIndex: 100,
              animation: 'fadeIn 0.15s ease-out',
            }}>
              {/* User Info Header */}
              <div style={{
                padding: '10px 12px',
                borderBottom: '1px solid var(--border-color)',
                marginBottom: '4px',
              }}>
                <div style={{ fontWeight: 600, fontSize: '13px', color: 'var(--text-primary)' }}>
                  {userData.fullName}
                </div>
                {userData.email && (
                  <div style={{ fontSize: '11.5px', color: 'var(--text-muted)', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                    {userData.email}
                  </div>
                )}
              </div>

              {/* Settings Link */}
              <button
                onClick={() => {
                  setDropdownOpen(false);
                  navigate('/settings');
                }}
                style={{
                  width: '100%',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '10px',
                  padding: '9px 12px',
                  borderRadius: '6px',
                  border: 'none',
                  backgroundColor: 'transparent',
                  color: 'var(--text-primary)',
                  fontSize: '13px',
                  cursor: 'pointer',
                  textAlign: 'left',
                  transition: 'background-color 0.15s',
                }}
                onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f8fafc'}
                onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
              >
                <Settings size={15} color="var(--text-secondary)" />
                <span>Settings</span>
              </button>

              {/* Logout Button */}
              <button
                onClick={handleLogout}
                style={{
                  width: '100%',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '10px',
                  padding: '9px 12px',
                  borderRadius: '6px',
                  border: 'none',
                  backgroundColor: 'transparent',
                  color: '#dc2626',
                  fontSize: '13px',
                  fontWeight: 500,
                  cursor: 'pointer',
                  textAlign: 'left',
                  transition: 'background-color 0.15s',
                }}
                onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#fef2f2'}
                onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
              >
                <LogOut size={15} color="#dc2626" />
                <span>Log Out</span>
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
