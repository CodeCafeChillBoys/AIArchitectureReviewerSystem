import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AlertCircle, CheckCircle2 } from 'lucide-react';
import { authService } from '../services/authService';

import AuthHeader from '../components/auth/AuthHeader';
import AuthTabs from '../components/auth/AuthTabs';
import LoginForm from '../components/auth/LoginForm';
import RegisterForm from '../components/auth/RegisterForm';

export default function LoginPage() {
  const navigate = useNavigate();

  // Mode: 'login' | 'register'
  const [mode, setMode] = useState('login');

  // Status State
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  // Xử lý Đăng nhập
  const handleLogin = async ({ email, password }) => {
    setErrorMessage('');
    setSuccessMessage('');

    try {
      setLoading(true);
      const response = await authService.login(email, password);
      if (response && response.success) {
        navigate('/dashboard');
      } else {
        setErrorMessage(response?.message || 'Đăng nhập không thành công.');
      }
    } catch (error) {
      const msg = error.response?.data?.message || error.message || 'Lỗi kết nối máy chủ.';
      setErrorMessage(msg);
    } finally {
      setLoading(false);
    }
  };

  // Xử lý Đăng ký
  const handleRegister = async ({ fullName, email, password }) => {
    setErrorMessage('');
    setSuccessMessage('');

    try {
      setLoading(true);
      const response = await authService.register(email, password, fullName);
      if (response && response.success) {
        setSuccessMessage('Đăng ký tài khoản thành công! Hãy chuyển sang đăng nhập.');
        setMode('login');
      } else {
        setErrorMessage(response?.message || 'Đăng ký không thành công.');
      }
    } catch (error) {
      const msg = error.response?.data?.message || error.message || 'Lỗi kết nối máy chủ.';
      setErrorMessage(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      minHeight: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: 'var(--bg-main)',
      padding: '24px',
    }}>
      <div style={{
        width: '100%',
        maxWidth: '440px',
        backgroundColor: '#ffffff',
        borderRadius: 'var(--radius-lg)',
        border: '1px solid var(--border-color)',
        boxShadow: '0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.03)',
        padding: '36px 32px',
      }}>
        {/* 1. Header (Logo & Tiêu đề) */}
        <AuthHeader
          subtitle={
            mode === 'login'
              ? 'Đăng nhập vào hệ thống review kiến trúc tự động'
              : 'Tạo tài khoản mới để bắt đầu thiết kế sơ đồ'
          }
        />

        {/* 2. Tabs chuyển đổi Đăng nhập / Đăng ký */}
        <AuthTabs
          activeTab={mode}
          onTabChange={(tab) => {
            setMode(tab);
            setErrorMessage('');
            setSuccessMessage('');
          }}
        />

        {/* 3. Thông báo Lỗi */}
        {errorMessage && (
          <div style={{
            padding: '10px 14px',
            backgroundColor: 'var(--color-danger-bg)',
            border: '1px solid #fecaca',
            borderRadius: 'var(--radius-sm)',
            color: 'var(--color-danger)',
            fontSize: '12.5px',
            display: 'flex',
            alignItems: 'center',
            gap: '8px',
            marginBottom: '20px',
          }}>
            <AlertCircle size={16} style={{ flexShrink: 0 }} />
            <span>{errorMessage}</span>
          </div>
        )}

        {/* 4. Thông báo Thành công */}
        {successMessage && (
          <div style={{
            padding: '10px 14px',
            backgroundColor: 'var(--color-success-bg)',
            border: '1px solid #bbf7d0',
            borderRadius: 'var(--radius-sm)',
            color: 'var(--color-success)',
            fontSize: '12.5px',
            display: 'flex',
            alignItems: 'center',
            gap: '8px',
            marginBottom: '20px',
          }}>
            <CheckCircle2 size={16} style={{ flexShrink: 0 }} />
            <span>{successMessage}</span>
          </div>
        )}

        {/* 5. Form tương ứng */}
        {mode === 'login' ? (
          <LoginForm onSubmit={handleLogin} loading={loading} />
        ) : (
          <RegisterForm onSubmit={handleRegister} loading={loading} />
        )}

        {/* 6. Footer chuyển đổi nhanh */}
        <div style={{ marginTop: '24px', textAlign: 'center', fontSize: '12.5px', color: 'var(--text-muted)' }}>
          {mode === 'login' ? (
            <span>
              Chưa có tài khoản?{' '}
              <button
                onClick={() => { setMode('register'); setErrorMessage(''); }}
                style={{ background: 'none', border: 'none', color: 'var(--accent-primary)', fontWeight: 600, cursor: 'pointer' }}
              >
                Đăng ký miễn phí
              </button>
            </span>
          ) : (
            <span>
              Đã có tài khoản?{' '}
              <button
                onClick={() => { setMode('login'); setErrorMessage(''); }}
                style={{ background: 'none', border: 'none', color: 'var(--accent-primary)', fontWeight: 600, cursor: 'pointer' }}
              >
                Đăng nhập
              </button>
            </span>
          )}
        </div>
      </div>
    </div>
  );
}
