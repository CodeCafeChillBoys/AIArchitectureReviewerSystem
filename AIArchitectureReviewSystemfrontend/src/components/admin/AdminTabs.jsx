import React from 'react';
import { BookOpen, MessageSquare, Users } from 'lucide-react';

export default function AdminTabs({ activeTab, onTabChange, rulesCount, promptsCount, usersCount }) {
  const tabs = [
    {
      id: 'rules',
      label: 'Quy tắc Kiến trúc (System Rules)',
      icon: BookOpen,
      count: rulesCount,
    },
    {
      id: 'prompts',
      label: 'Mẫu Prompts (Templates)',
      icon: MessageSquare,
      count: promptsCount,
    },
    {
      id: 'users',
      label: 'Quản lý Người dùng (Users)',
      icon: Users,
      count: usersCount,
    },
  ];

  return (
    <div style={{
      display: 'flex',
      gap: '8px',
      borderBottom: '1px solid var(--border-color)',
      marginBottom: '20px',
    }}>
      {tabs.map((tab) => {
        const Icon = tab.icon;
        const isActive = activeTab === tab.id;
        return (
          <button
            key={tab.id}
            onClick={() => onTabChange(tab.id)}
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              padding: '10px 16px',
              border: 'none',
              borderBottom: isActive ? '2px solid var(--accent-primary)' : '2px solid transparent',
              background: 'none',
              fontSize: '13.5px',
              fontWeight: isActive ? 600 : 500,
              color: isActive ? 'var(--accent-primary)' : 'var(--text-secondary)',
              cursor: 'pointer',
              transition: 'all 0.15s ease',
            }}
          >
            <Icon size={17} />
            <span>{tab.label}</span>
            <span style={{
              fontSize: '11px',
              padding: '1px 6px',
              borderRadius: '10px',
              backgroundColor: isActive ? 'var(--accent-blue-light)' : '#f1f5f9',
              color: isActive ? 'var(--accent-primary)' : '#64748b',
            }}>
              {tab.count}
            </span>
          </button>
        );
      })}
    </div>
  );
}
