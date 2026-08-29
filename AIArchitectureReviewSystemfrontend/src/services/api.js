import axios from 'axios';

// 1. Khởi tạo Axios instance với cấu hình mặc định
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  timeout: 30000, // 30s timeout
  headers: {
    'Content-Type': 'application/json',
  },
});

// 2. Request Interceptor: Tự động gắn Token vào Header nếu đã đăng nhập
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 3. Response Interceptor: Xử lý tập trung kết quả trả về và bắt các lỗi phổ biến
api.interceptors.response.use(
  (response) => {
    // Trả về trực tiếp data từ backend (không cần .data nhiều lần ở nơi gọi)
    return response.data;
  },
  (error) => {
    const { response } = error;

    if (response) {
      switch (response.status) {
        case 401:
          // Hết hạn phiên đăng nhập / Chưa xác thực
          console.error('Phiên đăng nhập đã hết hạn hoặc không hợp lệ.');
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('userId');
          localStorage.removeItem('user');
          if (window.location.pathname !== '/login') {
            window.location.href = '/login';
          }
          break;
        case 403:
          console.error('Bạn không có quyền truy cập tài nguyên này.');
          break;
        case 404:
          console.error('Không tìm thấy tài nguyên yêu cầu (404).');
          break;
        case 500:
          console.error('Lỗi hệ thống máy chủ (500).');
          break;
        default:
          console.error(response.data?.message || 'Đã xảy ra lỗi khi gọi API.');
      }
    } else if (error.code === 'ECONNABORTED') {
      console.error('Kết nối quá thời gian chờ (Timeout).');
    } else {
      console.error('Không thể kết nối tới máy chủ (Network Error).');
    }

    return Promise.reject(error);
  }
);

export default api;
