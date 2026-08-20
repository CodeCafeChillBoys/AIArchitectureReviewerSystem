import api from './api';

/**
 * Helper giải mã payload từ JWT Token mà không cần cài thêm thư viện
 */
export function parseJwt(token) {
  try {
    if (!token) return null;
    const base64Url = token.split('.')[1];
    if (!base64Url) return null;
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error('Lỗi khi decode JWT token:', e);
    return null;
  }
}

/**
 * Lưu thông tin phiên đăng nhập vào localStorage
 */
function saveAuthSession(data) {
  if (!data) return;

  const accessToken = data.accessToken || data.token;
  const refreshToken = data.refreshToken;
  const payload = parseJwt(accessToken);

  const userId =
    data.userId ||
    data.id ||
    data.user?.id ||
    data.user?.userId ||
    payload?.sub ||
    payload?.nameid ||
    payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

  const email =
    data.email ||
    data.user?.email ||
    payload?.email ||
    payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
    '';

  const role =
    data.role ||
    data.user?.role ||
    payload?.role ||
    payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
    'User';

  const fullName =
    data.fullName ||
    data.user?.fullName ||
    (email ? email.split('@')[0] : 'User');

  if (accessToken) localStorage.setItem('accessToken', accessToken);
  if (refreshToken) localStorage.setItem('refreshToken', refreshToken);
  if (userId) localStorage.setItem('userId', userId);

  localStorage.setItem(
    'user',
    JSON.stringify({
      id: userId,
      email,
      fullName,
      role,
    })
  );
}

export const authService = {
  /**
   * Đăng nhập bằng Email & Mật khẩu
   * POST /api/auth/login
   * Body: { email: string, password: string }
   */
  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    
    // Lưu Token và User Info vào localStorage khi thành công
    if (response && response.success && response.data) {
      saveAuthSession(response.data);
    } else if (response && response.data) {
      saveAuthSession(response.data);
    }
    return response;
  },

  /**
   * Đăng ký tài khoản mới
   * POST /api/auth/register
   * Body: { email: string, password: string, fullName: string }
   */
  register: async (email, password, fullName) => {
    return api.post('/auth/register', { email, password, fullName });
  },

  /**
   * Đăng nhập bằng Google
   * POST /api/auth/google-login
   * Body: { idToken: string }
   */
  googleLogin: async (idToken) => {
    const response = await api.post('/auth/google-login', { idToken });
    if (response && response.success && response.data) {
      saveAuthSession(response.data);
    } else if (response && response.data) {
      saveAuthSession(response.data);
    }
    return response;
  },

  /**
   * Đăng xuất: Xóa toàn bộ token & thông tin
   */
  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userId');
    localStorage.removeItem('user');
    window.location.href = '/login';
  },

  /**
   * Lấy User ID hiện tại (ưu tiên localStorage, fallback parse từ JWT token)
   */
  getUserId: () => {
    let userId = localStorage.getItem('userId');
    if (!userId) {
      const token = localStorage.getItem('accessToken');
      if (token) {
        const payload = parseJwt(token);
        userId =
          payload?.sub ||
          payload?.nameid ||
          payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
        if (userId) {
          localStorage.setItem('userId', userId);
        }
      }
    }
    return userId;
  },

  /**
   * Lấy thông tin user hiện tại
   */
  getCurrentUser: () => {
    const userStr = localStorage.getItem('user');
    try {
      if (userStr) return JSON.parse(userStr);
    } catch {
      // ignore
    }

    const token = localStorage.getItem('accessToken');
    if (token) {
      const payload = parseJwt(token);
      if (payload) {
        return {
          id: payload.sub || payload.nameid,
          email: payload.email || '',
          fullName: payload.email ? payload.email.split('@')[0] : 'User',
          role: payload.role || 'User',
        };
      }
    }
    return null;
  },

  /**
   * Kiểm tra đã đăng nhập chưa
   */
  isAuthenticated: () => {
    return !!localStorage.getItem('accessToken');
  },
};
