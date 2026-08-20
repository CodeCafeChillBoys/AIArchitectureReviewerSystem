import React from 'react';

export default function Badge({
  children,
  variant = 'neutral', // 'success' | 'warning' | 'danger' | 'neutral'
  className = '',
}) {
  return (
    <span className={`badge badge-${variant} ${className}`}>
      {children}
    </span>
  );
}
