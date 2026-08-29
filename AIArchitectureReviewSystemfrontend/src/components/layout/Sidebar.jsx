import React, { useState, useEffect } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import {
  LayoutDashboard,
  Sparkles,
  PenTool,
  Settings,
  FolderKanban,
  FileText,
  HelpCircle,
  Plus,
  Zap,
  Shield,
} from 'lucide-react';
import { authService } from '../../services/authService';
import { workspaceService } from '../../services/workspaceService';

export default function Sidebar() {
  const navigate = useNavigate();
  const [recentWorkspaces, setRecentWorkspaces] = useState([]);

  useEffect(() => {
    const fetchRecentWorkspaces = async () => {
      const userId = authService.getUserId();
      if (!userId) return;
      try {
        const res = await workspaceService.getUserWorkspaces(userId, 1, 4);
        const items = res?.data?.items || res?.data || res || [];
        if (Array.isArray(items)) {
          setRecentWorkspaces(items.slice(0, 4));
        }
      } catch (err) {
        // Silently catch to not break sidebar
      }
    };
    fetchRecentWorkspaces();
  }, []);

  return (
    <aside className="sidebar" style={{
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'space-between',
      height: '100vh',
      padding: '16px 14px',
      backgroundColor: '#ffffff',
      borderRight: '1px solid var(--border-color)',
      boxSizing: 'border-box',
    }}>
      {/* Top Half: Logo & Navigation */}
      <div>
        {/* 1. Logo & Tên ứng dụng */}
        <NavLink
          to="/dashboard"
          className="sidebar-logo"
          style={{
            display: 'flex',
            alignItems: 'center',
            gap: '10px',
            textDecoration: 'none',
            padding: '6px 8px',
            marginBottom: '16px',
          }}
        >
          <img
            src="/logo.png"
            alt="AI Diagram Logo"
            style={{
              width: '32px',
              height: '32px',
              objectFit: 'contain',
              borderRadius: '8px',
              filter: 'drop-shadow(0 2px 4px rgba(37, 99, 235, 0.2))',
            }}
          />
          <div className="sidebar-title" style={{ display: 'flex', flexDirection: 'column' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <span style={{ fontSize: '14.5px', fontWeight: 700, color: 'var(--text-primary)' }}>AI Diagram</span>
              <span style={{
                fontSize: '9.5px',
                fontWeight: 700,
                padding: '1px 5px',
                borderRadius: '6px',
                backgroundColor: 'var(--accent-blue-light)',
                color: 'var(--accent-primary)',
              }}>
                v1.2
              </span>
            </div>
            <span style={{ fontSize: '11px', color: 'var(--text-muted)' }}>Review System</span>
          </div>
        </NavLink>

        {/* 2. Menu Điều Hướng Chính */}
        <nav className="nav-section" style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
          <NavLink
            to="/dashboard"
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
            style={{ display: 'flex', alignItems: 'center', gap: '10px', padding: '9px 12px', borderRadius: '8px', textDecoration: 'none', fontSize: '13px' }}
          >
            <LayoutDashboard size={17} />
            <span>Dashboard</span>
          </NavLink>

          {authService.isAdmin() && (
            <NavLink
              to="/admin"
              className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
              style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '9px 12px', borderRadius: '8px', textDecoration: 'none', fontSize: '13px' }}
            >
              <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                <Shield size={17} style={{ color: '#7c3aed' }} />
                <span>Admin Console</span>
              </div>
              <span style={{
                fontSize: '10px',
                fontWeight: 700,
                padding: '1px 5px',
                borderRadius: '4px',
                backgroundColor: '#f5f3ff',
                color: '#7c3aed',
              }}>
                HUB
              </span>
            </NavLink>
          )}

          <NavLink
            to="/settings"
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
            style={{ display: 'flex', alignItems: 'center', gap: '10px', padding: '9px 12px', borderRadius: '8px', textDecoration: 'none', fontSize: '13px' }}
          >
            <Settings size={17} />
            <span>Settings</span>
          </NavLink>
        </nav>

        {/* 3. Phân vùng Recent Workspaces */}
        <div style={{ marginTop: '24px' }}>
          <div style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            padding: '0 8px',
            marginBottom: '8px',
          }}>
            <span style={{
              fontSize: '11px',
              fontWeight: 600,
              color: 'var(--text-muted)',
              textTransform: 'uppercase',
              letterSpacing: '0.04em',
            }}>
              Recent Workspaces
            </span>
            <button
              onClick={() => navigate('/dashboard')}
              title="View all workspaces"
              style={{
                background: 'transparent',
                border: 'none',
                color: 'var(--accent-primary)',
                cursor: 'pointer',
                padding: '2px',
                display: 'flex',
                alignItems: 'center',
              }}
            >
              <Plus size={14} />
            </button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '2px' }}>
            {recentWorkspaces.length > 0 ? (
              recentWorkspaces.map((ws) => (
                <NavLink
                  key={ws.id}
                  to={`/workspace/${ws.id}`}
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: '8px',
                    padding: '7px 10px',
                    borderRadius: '6px',
                    fontSize: '12.5px',
                    color: 'var(--text-secondary)',
                    textDecoration: 'none',
                    transition: 'all 0.15s ease',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                  }}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.backgroundColor = '#f8fafc';
                    e.currentTarget.style.color = 'var(--accent-primary)';
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.backgroundColor = 'transparent';
                    e.currentTarget.style.color = 'var(--text-secondary)';
                  }}
                >
                  <FolderKanban size={14} style={{ flexShrink: 0 }} />
                  <span style={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {ws.name}
                  </span>
                </NavLink>
              ))
            ) : (
              <div style={{ padding: '6px 10px', fontSize: '12px', color: 'var(--text-muted)' }}>
                No active workspaces
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Bottom Half: Footer Support */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '14px', marginTop: '16px' }}>
        {/* Chân trang Sidebar (Documentation & Support) */}
        <div style={{
          borderTop: '1px solid var(--border-color)',
          paddingTop: '12px',
          display: 'flex',
          flexDirection: 'column',
          gap: '4px',
        }}>
          <a
            href="#docs"
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              padding: '6px 10px',
              borderRadius: '6px',
              fontSize: '12.5px',
              color: 'var(--text-secondary)',
              textDecoration: 'none',
              transition: 'background-color 0.15s',
            }}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f8fafc'}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
          >
            <FileText size={15} />
            <span>Documentation</span>
          </a>

          <a
            href="#support"
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              padding: '6px 10px',
              borderRadius: '6px',
              fontSize: '12.5px',
              color: 'var(--text-secondary)',
              textDecoration: 'none',
              transition: 'background-color 0.15s',
            }}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f8fafc'}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
          >
            <HelpCircle size={15} />
            <span>Support</span>
          </a>
        </div>
      </div>
    </aside>
  );
}
