import React from 'react';

export default function Button({
  children,
  variant = 'primary', // 'primary' | 'secondary' | 'outline' | 'danger'
  size = 'md',        // 'sm' | 'md' | 'lg'
  icon: Icon,
  className = '',
  ...props
}) {
  const sizeClass = size === 'sm' ? 'btn-sm' : '';
  const variantClass = `btn-${variant}`;

  return (
    <button className={`btn ${variantClass} ${sizeClass} ${className}`} {...props}>
      {Icon && <Icon size={size === 'sm' ? 14 : 16} />}
      {children}
    </button>
  );
}
