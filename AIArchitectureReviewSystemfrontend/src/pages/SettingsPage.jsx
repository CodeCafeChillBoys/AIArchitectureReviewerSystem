import React, { useState } from 'react';
import {
  User,
  Save,
  CheckCircle2,
  LogOut,
  Mail,
  Briefcase,
  FileText,
} from 'lucide-react';
import { authService } from '../services/authService';

export default function SettingsPage() {
  const currentUser = authService.getCurrentUser() || {
    fullName: 'System Architect',
    email: 'architect@enterprise.io',
    role: 'Software Architect',
  };

  const [fullName, setFullName] = useState(currentUser.fullName || '');
  const [email] = useState(currentUser.email || '');
  const [role, setRole] = useState(currentUser.role || 'Software Architect');
  const [bio, setBio] = useState(
    'Lead Software Architect focused on Microservices, Clean Architecture, and Event-Driven Systems.'
  );
  const [savedSuccess, setSavedSuccess] = useState(false);

  const handleSaveSettings = (e) => {
    e?.preventDefault();

    // Persist to localStorage
    const updatedUser = {
      ...currentUser,
      fullName,
      role,
    };
    localStorage.setItem('user', JSON.stringify(updatedUser));

    setSavedSuccess(true);
    setTimeout(() => setSavedSuccess(false), 2500);
  };

  return (
    <div style={{ padding: '32px 36px', maxWidth: '800px', margin: '0 auto' }}>
      {/* Header */}
      <div style={{ marginBottom: '28px' }}>
        <h1 style={{ fontSize: '24px', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '4px' }}>
          Account Settings
        </h1>
        <p style={{ fontSize: '13px', color: 'var(--text-muted)' }}>
          Manage your personal profile and account details.
        </p>
      </div>

      {/* Main Settings Card */}
      <div className="card" style={{ padding: '32px', backgroundColor: '#ffffff' }}>
        {/* Header inside card */}
        <div style={{ paddingBottom: '16px', borderBottom: '1px solid var(--border-color)', marginBottom: '24px' }}>
          <h3 style={{ fontSize: '16px', fontWeight: 700, color: 'var(--text-primary)', margin: 0 }}>
            Profile Information
          </h3>
          <p style={{ fontSize: '12.5px', color: 'var(--text-muted)', marginTop: '4px' }}>
            Update your display name, role, and public bio.
          </p>
        </div>

        {/* Avatar section */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '18px', marginBottom: '28px' }}>
          <div style={{
            width: '64px',
            height: '64px',
            borderRadius: '50%',
            backgroundColor: 'var(--accent-primary)',
            color: '#ffffff',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: '22px',
            fontWeight: 700,
            boxShadow: '0 4px 12px rgba(37, 99, 235, 0.25)',
          }}>
            {fullName ? fullName.charAt(0).toUpperCase() : 'U'}
          </div>
          <div>
            <h4 style={{ fontSize: '15px', fontWeight: 600, color: 'var(--text-primary)', margin: 0 }}>
              {fullName || 'User'}
            </h4>
            <span style={{ fontSize: '12.5px', color: 'var(--text-muted)' }}>{email}</span>
          </div>
        </div>

        {/* Profile Form Fields */}
        <form onSubmit={handleSaveSettings} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
          <div>
            <label style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '6px' }}>
              <User size={15} color="var(--accent-primary)" />
              <span>Full Name</span>
            </label>
            <input
              type="text"
              className="input-text"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              placeholder="Enter your full name"
              style={{ width: '100%', height: '38px', fontSize: '13.5px' }}
            />
          </div>

          <div>
            <label style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '6px' }}>
              <Mail size={15} color="var(--accent-primary)" />
              <span>Email Address</span>
            </label>
            <input
              type="email"
              className="input-text"
              value={email}
              disabled
              style={{ width: '100%', height: '38px', fontSize: '13.5px', backgroundColor: '#f8fafc', color: 'var(--text-muted)' }}
            />
          </div>

          <div>
            <label style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '6px' }}>
              <Briefcase size={15} color="var(--accent-primary)" />
              <span>Professional Role</span>
            </label>
            <select
              className="input-text"
              value={role}
              onChange={(e) => setRole(e.target.value)}
              style={{ width: '100%', height: '38px', fontSize: '13.5px', cursor: 'pointer' }}
            >
              <option value="Enterprise Architect">Enterprise Architect</option>
              <option value="Software Architect">Software Architect</option>
              <option value="Senior Tech Lead">Senior Tech Lead</option>
              <option value="Backend Engineer">Backend Engineer</option>
              <option value="Systems Engineer">Systems Engineer</option>
            </select>
          </div>

          <div>
            <label style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px', fontWeight: 600, color: 'var(--text-primary)', marginBottom: '6px' }}>
              <FileText size={15} color="var(--accent-primary)" />
              <span>Bio / Description</span>
            </label>
            <textarea
              rows={3}
              className="input-text"
              value={bio}
              onChange={(e) => setBio(e.target.value)}
              placeholder="A brief description of your technical focus..."
              style={{ width: '100%', padding: '10px 12px', fontSize: '13px', resize: 'vertical' }}
            />
          </div>

          {/* Action Bar */}
          <div style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            marginTop: '20px',
            paddingTop: '20px',
            borderTop: '1px solid var(--border-color)',
          }}>
            <div>
              {savedSuccess && (
                <div style={{
                  display: 'inline-flex',
                  alignItems: 'center',
                  gap: '6px',
                  color: '#16a34a',
                  fontSize: '13px',
                  fontWeight: 600,
                }}>
                  <CheckCircle2 size={16} />
                  <span>Profile updated successfully!</span>
                </div>
              )}
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
              <button
                type="button"
                className="btn btn-outline btn-sm"
                onClick={() => authService.logout()}
                style={{
                  display: 'inline-flex',
                  alignItems: 'center',
                  gap: '6px',
                  color: '#dc2626',
                  borderColor: '#fecaca',
                  height: '38px',
                  padding: '0 14px',
                }}
              >
                <LogOut size={14} />
                <span>Log Out</span>
              </button>

              <button
                type="submit"
                className="btn btn-primary"
                style={{
                  display: 'inline-flex',
                  alignItems: 'center',
                  gap: '8px',
                  height: '38px',
                  padding: '0 20px',
                  fontWeight: 600,
                }}
              >
                <Save size={15} />
                <span>Save Changes</span>
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
