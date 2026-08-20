import React from 'react';

export default function AuthTabs({ activeTab, onTabChange }) {
  return (
    <div style={{
      display: 'flex',
      backgroundColor: '#f1f5f9',
      borderRadius: 'var(--radius-md)',
      padding: '4px',
      marginBottom: '24px',
    }}>
      <button
        type="button"
        onClick={() => onTabChange('login')}
        style={{
          flex: 1,
          padding: '8px',
          border: 'none',
          borderRadius: 'var(--radius-sm)',
          fontSize: '13px',
          fontWeight: 600,
          cursor: 'pointer',
          transition: 'all 0.15s ease',
          backgroundColor: activeTab === 'login' ? '#ffffff' : 'transparent',
          color: activeTab === 'login' ? 'var(--accent-primary)' : 'var(--text-secondary)',
          boxShadow: activeTab === 'login' ? '0 1px 3px rgba(0,0,0,0.08)' : 'none',
        }}
      >
        Sign In
      </button>
      <button
        type="button"
        onClick={() => onTabChange('register')}
        style={{
          flex: 1,
          padding: '8px',
          border: 'none',
          borderRadius: 'var(--radius-sm)',
          fontSize: '13px',
          fontWeight: 600,
          cursor: 'pointer',
          transition: 'all 0.15s ease',
          backgroundColor: activeTab === 'register' ? '#ffffff' : 'transparent',
          color: activeTab === 'register' ? 'var(--accent-primary)' : 'var(--text-secondary)',
          boxShadow: activeTab === 'register' ? '0 1px 3px rgba(0,0,0,0.08)' : 'none',
        }}
      >
        Sign Up
      </button>
    </div>
  );
}
