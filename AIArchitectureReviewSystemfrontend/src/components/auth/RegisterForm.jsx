import React, { useState } from 'react';
import { User, Mail, Lock, Eye, EyeOff, ArrowRight, Loader2 } from 'lucide-react';

export default function RegisterForm({ onSubmit, loading }) {
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit({ fullName, email, password });
  };

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
      {/* Full Name */}
      <div>
        <label style={{ display: 'block', fontSize: '13px', fontWeight: 500, color: 'var(--text-secondary)', marginBottom: '6px' }}>
          Full Name
        </label>
        <div style={{ position: 'relative' }}>
          <User size={16} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
          <input
            type="text"
            className="input-text"
            placeholder="John Doe"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            style={{ paddingLeft: '36px' }}
            required
          />
        </div>
      </div>

      {/* Email */}
      <div>
        <label style={{ display: 'block', fontSize: '13px', fontWeight: 500, color: 'var(--text-secondary)', marginBottom: '6px' }}>
          Email Address
        </label>
        <div style={{ position: 'relative' }}>
          <Mail size={16} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
          <input
            type="email"
            className="input-text"
            placeholder="name@company.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            style={{ paddingLeft: '36px' }}
            required
          />
        </div>
      </div>

      {/* Password */}
      <div>
        <label style={{ display: 'block', fontSize: '13px', fontWeight: 500, color: 'var(--text-secondary)', marginBottom: '6px' }}>
          Password
        </label>
        <div style={{ position: 'relative' }}>
          <Lock size={16} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
          <input
            type={showPassword ? 'text' : 'password'}
            className="input-text"
            placeholder="••••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{ paddingLeft: '36px', paddingRight: '36px' }}
            required
          />
          <button
            type="button"
            onClick={() => setShowPassword(!showPassword)}
            style={{
              position: 'absolute',
              right: '10px',
              top: '50%',
              transform: 'translateY(-50%)',
              background: 'none',
              border: 'none',
              cursor: 'pointer',
              color: 'var(--text-muted)',
            }}
          >
            {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
          </button>
        </div>
      </div>

      {/* Submit Button */}
      <button
        type="submit"
        className="btn btn-primary"
        disabled={loading}
        style={{ width: '100%', padding: '10px', marginTop: '6px', fontSize: '14px' }}
      >
        {loading ? (
          <>
            <Loader2 size={16} className="animate-spin" />
            <span>Creating account...</span>
          </>
        ) : (
          <>
            <span>Create Account</span>
            <ArrowRight size={16} />
          </>
        )}
      </button>
    </form>
  );
}
