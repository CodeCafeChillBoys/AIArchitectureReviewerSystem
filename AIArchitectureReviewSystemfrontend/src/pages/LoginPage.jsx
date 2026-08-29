import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { AlertCircle, CheckCircle2 } from 'lucide-react';
import { authService } from '../services/authService';

import AuthHeader from '../components/auth/AuthHeader';
import AuthTabs from '../components/auth/AuthTabs';
import LoginForm from '../components/auth/LoginForm';
import RegisterForm from '../components/auth/RegisterForm';

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || '/dashboard';

  // Mode: 'login' | 'register'
  const [mode, setMode] = useState('login');

  // Status State
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  // Nếu đã đăng nhập rồi thì điều hướng thẳng tới dashboard hoặc route trước đó
  useEffect(() => {
    if (authService.isAuthenticated()) {
      navigate(from, { replace: true });
    }
  }, [navigate, from]);

  // Handle Login
  const handleLogin = async ({ email, password }) => {
    setErrorMessage('');
    setSuccessMessage('');

    try {
      setLoading(true);
      const response = await authService.login(email, password);
      if (response && response.success) {
        navigate(from, { replace: true });
      } else {
        setErrorMessage(response?.message || 'Login failed. Please check credentials.');
      }
    } catch (error) {
      const msg = error.response?.data?.message || error.message || 'Server connection error.';
      setErrorMessage(msg);
    } finally {
      setLoading(false);
    }
  };

  // Handle Register
  const handleRegister = async ({ fullName, email, password }) => {
    setErrorMessage('');
    setSuccessMessage('');

    try {
      setLoading(true);
      const response = await authService.register(email, password, fullName);
      if (response && response.success) {
        setSuccessMessage('Registration successful! Please log in.');
        setMode('login');
      } else {
        setErrorMessage(response?.message || 'Registration failed.');
      }
    } catch (error) {
      const msg = error.response?.data?.message || error.message || 'Server connection error.';
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
        {/* 1. Header */}
        <AuthHeader
          subtitle={
            mode === 'login'
              ? 'Sign in to access your architecture review dashboard'
              : 'Create a new account to start reviewing diagrams'
          }
        />

        {/* 2. Tabs */}
        <AuthTabs
          activeTab={mode}
          onTabChange={(tab) => {
            setMode(tab);
            setErrorMessage('');
            setSuccessMessage('');
          }}
        />

        {/* 3. Error Alert */}
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

        {/* 4. Success Alert */}
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

        {/* 5. Form */}
        {mode === 'login' ? (
          <LoginForm onSubmit={handleLogin} loading={loading} />
        ) : (
          <RegisterForm onSubmit={handleRegister} loading={loading} />
        )}

        {/* 6. Footer */}
        <div style={{ marginTop: '24px', textAlign: 'center', fontSize: '12.5px', color: 'var(--text-muted)' }}>
          {mode === 'login' ? (
            <span>
              Don't have an account?{' '}
              <button
                onClick={() => { setMode('register'); setErrorMessage(''); }}
                style={{ background: 'none', border: 'none', color: 'var(--accent-primary)', fontWeight: 600, cursor: 'pointer' }}
              >
                Sign up free
              </button>
            </span>
          ) : (
            <span>
              Already have an account?{' '}
              <button
                onClick={() => { setMode('login'); setErrorMessage(''); }}
                style={{ background: 'none', border: 'none', color: 'var(--accent-primary)', fontWeight: 600, cursor: 'pointer' }}
              >
                Sign in
              </button>
            </span>
          )}
        </div>
      </div>
    </div>
  );
}
