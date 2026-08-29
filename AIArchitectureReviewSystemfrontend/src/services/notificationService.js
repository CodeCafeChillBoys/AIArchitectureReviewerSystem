/**
 * notificationService.js
 * Quản lý lưu trữ & sự kiện thông báo realtime.
 */

const STORAGE_KEY_PREFIX = 'ai_arch_notifications_';

const getStorageKey = () => {
  try {
    const user = JSON.parse(localStorage.getItem('user'));
    return `${STORAGE_KEY_PREFIX}${user?.id || 'guest'}`;
  } catch {
    return `${STORAGE_KEY_PREFIX}guest`;
  }
};

export const notificationService = {
  getNotifications: () => {
    try {
      const key = getStorageKey();
      const raw = localStorage.getItem(key);
      if (!raw) {
        return [];
      }
      return JSON.parse(raw);
    } catch {
      return [];
    }
  },

  addNotification: ({
    type = 'ai_review',
    title = 'AI Analysis Completed',
    message = 'Diagram evaluation has been completed.',
    diagramId = null,
    workspaceId = null,
    score = null,
  }) => {
    const key = getStorageKey();
    const current = notificationService.getNotifications();
    const newNotif = {
      id: 'notif-' + Date.now() + '-' + Math.random().toString(36).substr(2, 5),
      type,
      title,
      message,
      diagramId,
      workspaceId,
      score,
      timestamp: new Date().toISOString(),
      isRead: false,
    };

    const updated = [newNotif, ...current];
    localStorage.setItem(key, JSON.stringify(updated));
    window.dispatchEvent(new CustomEvent('ai_notifications_changed', { detail: updated }));
    return newNotif;
  },

  markAsRead: (id) => {
    const key = getStorageKey();
    const current = notificationService.getNotifications();
    const updated = current.map((n) => (n.id === id ? { ...n, isRead: true } : n));
    localStorage.setItem(key, JSON.stringify(updated));
    window.dispatchEvent(new CustomEvent('ai_notifications_changed', { detail: updated }));
  },

  markAllAsRead: () => {
    const key = getStorageKey();
    const current = notificationService.getNotifications();
    const updated = current.map((n) => ({ ...n, isRead: true }));
    localStorage.setItem(key, JSON.stringify(updated));
    window.dispatchEvent(new CustomEvent('ai_notifications_changed', { detail: updated }));
  },

  clearAll: () => {
    const key = getStorageKey();
    localStorage.setItem(key, JSON.stringify([]));
    window.dispatchEvent(new CustomEvent('ai_notifications_changed', { detail: [] }));
  },

  getUnreadCount: () => {
    const notifs = notificationService.getNotifications();
    return notifs.filter((n) => !n.isRead).length;
  },
};
