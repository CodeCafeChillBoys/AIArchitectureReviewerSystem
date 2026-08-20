import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  Settings,
  FileText,
  HelpCircle,
} from 'lucide-react';

const navItems = [
  { name: 'Dashboard', path: '/dashboard', icon: LayoutDashboard },
  { name: 'Settings', path: '/settings', icon: Settings },
];

export default function Sidebar() {
  return (
    <aside className="sidebar">
      {/* 1. Logo & Tên ứng dụng */}
      <NavLink to="/dashboard" className="sidebar-logo">
        <img
          src="/logo.png"
          alt="AI Diagram Grading System Logo"
          style={{
            width: '36px',
            height: '36px',
            objectFit: 'contain',
            borderRadius: '8px',
            filter: 'drop-shadow(0 2px 4px rgba(37, 99, 235, 0.2))',
          }}
        />
        <div className="sidebar-title">
          AI Diagram
          <span>Review System</span>
        </div>
      </NavLink>

      {/* 2. Danh sách Menu điều hướng */}
      <nav className="nav-section" style={{ marginTop: '16px' }}>
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
            >
              <Icon size={18} />
              <span>{item.name}</span>
            </NavLink>
          );
        })}
      </nav>

      {/* 4. Chân trang Sidebar (Documentation & Support) */}
      <div style={{
        borderTop: '1px solid var(--border-subtle)',
        paddingTop: '16px',
        display: 'flex',
        flexDirection: 'column',
        gap: '8px'
      }}>
        <a href="#docs" className="nav-link" style={{ padding: '8px 12px' }}>
          <FileText size={16} />
          <span style={{ fontSize: '12.5px' }}>Documentation</span>
        </a>
        <a href="#support" className="nav-link" style={{ padding: '8px 12px' }}>
          <HelpCircle size={16} />
          <span style={{ fontSize: '12.5px' }}>Support</span>
        </a>
      </div>
    </aside>
  );
}

