import React from 'react';

export default function AuthHeader({ subtitle }) {
  return (
    <div style={{ textAlign: 'center', marginBottom: '28px' }}>
      <div style={{
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '12px',
      }}>
        <img
          src="/logo.png"
          alt="AI Diagram Grading System Logo"
          style={{
            width: '64px',
            height: '64px',
            objectFit: 'contain',
            borderRadius: '12px',
            filter: 'drop-shadow(0 4px 12px rgba(37, 99, 235, 0.25))',
          }}
        />
      </div>
      <h1 style={{ fontSize: '20px', fontWeight: 700, color: 'var(--text-primary)' }}>
        AI Diagram Grading System
      </h1>
      <p style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>
        {subtitle}
      </p>
    </div>
  );
}
