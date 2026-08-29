import React, { useState, useEffect, useRef } from 'react';
import {
  Bell,
  CheckCheck,
  Trash2,
  ChevronRight,
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { notificationService } from '../../services/notificationService';

export default function NotificationDropdown() {
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const dropdownRef = useRef(null);

  const refreshData = () => {
    const list = notificationService.getNotifications();
    setNotifications(list);
    setUnreadCount(list.filter((n) => !n.isRead).length);
  };

  useEffect(() => {
    refreshData();

    const handleUpdate = () => refreshData();
    window.addEventListener('ai_notifications_changed', handleUpdate);
    window.addEventListener('storage', handleUpdate);

    return () => {
      window.removeEventListener('ai_notifications_changed', handleUpdate);
      window.removeEventListener('storage', handleUpdate);
    };
  }, []);

  // Close dropdown when click outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleItemClick = (item) => {
    notificationService.markAsRead(item.id);
    setIsOpen(false);
    if (item.diagramId) {
      navigate(`/review/${item.diagramId}`);
    }
  };

  const handleMarkAll = () => {
    notificationService.markAllAsRead();
  };

  const handleClearAll = () => {
    notificationService.clearAll();
  };

  const formatTime = (isoString) => {
    if (!isoString) return 'Just now';
    const d = new Date(isoString);
    const now = new Date();
    const diffSec = Math.floor((now - d) / 1000);

    if (diffSec < 60) return 'Just now';
    if (diffSec < 3600) return `${Math.floor(diffSec / 60)}m ago`;
    if (diffSec < 86400) return `${Math.floor(diffSec / 3600)}h ago`;
    return d.toLocaleDateString();
  };

  return (
    <div style={{ position: 'relative' }} ref={dropdownRef}>
      {/* Bell Button */}
      <button
        onClick={() => setIsOpen((prev) => !prev)}
        className="btn btn-outline btn-sm"
        title="Notifications"
        style={{
          padding: '8px',
          borderRadius: '50%',
          position: 'relative',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          backgroundColor: isOpen ? 'var(--accent-blue-light)' : '#ffffff',
          borderColor: isOpen ? 'var(--accent-primary)' : 'var(--border-color)',
          transition: 'all 0.2s ease',
        }}
      >
        <Bell size={16} color={isOpen ? 'var(--accent-primary)' : 'var(--text-primary)'} />

        {/* Red Unread Badge */}
        {unreadCount > 0 && (
          <span
            style={{
              position: 'absolute',
              top: '-4px',
              right: '-4px',
              backgroundColor: '#ef4444',
              color: '#ffffff',
              fontSize: '10px',
              fontWeight: 700,
              minWidth: '18px',
              height: '18px',
              borderRadius: '9px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              padding: '0 4px',
              boxShadow: '0 0 0 2px #ffffff',
              animation: 'pulse 2s infinite',
            }}
          >
            {unreadCount > 99 ? '99+' : unreadCount}
          </span>
        )}
      </button>

      {/* Dropdown Panel */}
      {isOpen && (
        <div
          style={{
            position: 'absolute',
            top: 'calc(100% + 10px)',
            right: 0,
            width: '380px',
            maxWidth: '90vw',
            backgroundColor: '#ffffff',
            borderRadius: 'var(--radius-lg)',
            border: '1px solid var(--border-color)',
            boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
            zIndex: 1000,
            overflow: 'hidden',
            animation: 'fadeIn 0.15s ease-out',
          }}
        >
          {/* Header */}
          <div
            style={{
              padding: '14px 18px',
              borderBottom: '1px solid var(--border-color)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              backgroundColor: '#fafafa',
            }}
          >
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
              <span style={{ fontWeight: 600, fontSize: '14px', color: 'var(--text-primary)' }}>
                Notifications
              </span>
              {unreadCount > 0 && (
                <span
                  style={{
                    backgroundColor: 'var(--accent-blue-light)',
                    color: 'var(--accent-primary)',
                    fontSize: '11px',
                    fontWeight: 600,
                    padding: '2px 8px',
                    borderRadius: '10px',
                  }}
                >
                  {unreadCount} new
                </span>
              )}
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              {unreadCount > 0 && (
                <button
                  onClick={handleMarkAll}
                  style={{
                    border: 'none',
                    background: 'none',
                    color: 'var(--accent-primary)',
                    fontSize: '12px',
                    cursor: 'pointer',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '4px',
                    padding: '4px 6px',
                    borderRadius: '4px',
                    fontWeight: 500,
                  }}
                  title="Mark all as read"
                >
                  <CheckCheck size={14} />
                  <span>Mark all read</span>
                </button>
              )}
              {notifications.length > 0 && (
                <button
                  onClick={handleClearAll}
                  style={{
                    border: 'none',
                    background: 'none',
                    color: 'var(--text-muted)',
                    fontSize: '12px',
                    cursor: 'pointer',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '4px',
                    padding: '4px 6px',
                    borderRadius: '4px',
                  }}
                  title="Clear all notifications"
                >
                  <Trash2 size={13} />
                </button>
              )}
            </div>
          </div>

          {/* List Content */}
          <div style={{ maxHeight: '380px', overflowY: 'auto' }}>
            {notifications.length === 0 ? (
              <div
                style={{
                  padding: '36px 20px',
                  textAlign: 'center',
                  color: 'var(--text-muted)',
                }}
              >
                <div
                  style={{
                    width: '44px',
                    height: '44px',
                    borderRadius: '50%',
                    backgroundColor: '#f1f5f9',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    margin: '0 auto 10px',
                    color: '#94a3b8',
                  }}
                >
                  <Bell size={20} />
                </div>
                <div style={{ fontSize: '13.5px', fontWeight: 500, color: 'var(--text-primary)', marginBottom: '2px' }}>
                  No notifications yet
                </div>
                <div style={{ fontSize: '12px' }}>
                  When AI finishes analyzing your diagrams, you'll be notified here.
                </div>
              </div>
            ) : (
              notifications.map((item) => {
                const isRead = item.isRead;
                return (
                  <div
                    key={item.id}
                    onClick={() => handleItemClick(item)}
                    style={{
                      padding: '12px 16px',
                      borderBottom: '1px solid var(--border-color)',
                      backgroundColor: isRead ? '#ffffff' : '#f0f7ff',
                      cursor: 'pointer',
                      display: 'flex',
                      alignItems: 'flex-start',
                      gap: '12px',
                      transition: 'background-color 0.15s ease',
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.backgroundColor = isRead ? '#f8fafc' : '#e0efff';
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.backgroundColor = isRead ? '#ffffff' : '#f0f7ff';
                    }}
                  >
                    {/* Body */}
                    <div style={{ flex: 1, minWidth: 0 }}>
                      <div
                        style={{
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'space-between',
                          gap: '6px',
                          marginBottom: '2px',
                        }}
                      >
                        <span
                          style={{
                            fontSize: '13px',
                            fontWeight: isRead ? 500 : 700,
                            color: 'var(--text-primary)',
                            overflow: 'hidden',
                            textOverflow: 'ellipsis',
                            whiteSpace: 'nowrap',
                          }}
                        >
                          {item.title}
                        </span>
                        <span style={{ fontSize: '11px', color: 'var(--text-muted)', whiteSpace: 'nowrap' }}>
                          {formatTime(item.timestamp)}
                        </span>
                      </div>

                      <p
                        style={{
                          fontSize: '12px',
                          color: 'var(--text-secondary)',
                          lineHeight: 1.4,
                          margin: '0 0 6px 0',
                          display: '-webkit-box',
                          WebkitLineClamp: 2,
                          WebkitBoxOrient: 'vertical',
                          overflow: 'hidden',
                        }}
                      >
                        {item.message}
                      </p>

                      {item.diagramId && (
                        <div
                          style={{
                            display: 'inline-flex',
                            alignItems: 'center',
                            gap: '4px',
                            fontSize: '11.5px',
                            color: 'var(--accent-primary)',
                            fontWeight: 600,
                          }}
                        >
                          <span>View Review Report</span>
                          <ChevronRight size={12} />
                        </div>
                      )}
                    </div>

                    {/* Blue Unread Dot */}
                    {!isRead && (
                      <div
                        style={{
                          width: '8px',
                          height: '8px',
                          borderRadius: '50%',
                          backgroundColor: '#2563eb',
                          marginTop: '6px',
                          flexShrink: 0,
                        }}
                      />
                    )}
                  </div>
                );
              })
            )}
          </div>
        </div>
      )}
    </div>
  );
}
