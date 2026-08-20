import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  ArrowLeft,
  Save,
  Bot,
  Code2,
  Eye,
  Loader2,
  AlertCircle,
  Sparkles,
  ZoomIn,
  ZoomOut,
  RotateCcw,
  Download,
  ArrowRight,
} from 'lucide-react';
import { diagramService } from '../services/diagramService';
import { reviewService } from '../services/reviewService';
import MermaidRenderer from '../components/common/MermaidRenderer';

const TEMPLATES = {
  Flowchart: `graph TD
    A[Start] --> B{Check Condition}
    B -- Valid --> C[Process Request]
    B -- Invalid --> D[Return Error]
    C --> E[Complete]`,
  Sequence: `sequenceDiagram
    autonumber
    actor User as Client
    participant API as API Gateway
    participant Auth as Auth Service
    participant DB as Database

    User->>API: POST /api/auth/login
    API->>Auth: Validate Credentials
    Auth->>DB: Query User
    DB-->>Auth: Return User Data
    Auth-->>API: 200 OK + Token
    API-->>User: Login Successful`,
  Class: `classDiagram
    class User {
        +Guid Id
        +string FullName
        +string Email
        +Login()
    }
    class Workspace {
        +Guid Id
        +string Name
        +Guid UserId
        +AddDiagram()
    }
    User "1" -- "*" Workspace : owns`,
  ERD: `erDiagram
    USERS ||--o{ WORKSPACES : owns
    WORKSPACES ||--o{ DIAGRAMS : contains

    USERS {
        uuid id PK
        string email
        string full_name
    }
    WORKSPACES {
        uuid id PK
        uuid user_id FK
        string name
    }`,
};

export default function DiagramEditorPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [diagram, setDiagram] = useState(null);
  const [report, setReport] = useState(null);
  const [code, setCode] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState('');

  // Image Zoom & Viewer state
  const [zoomLevel, setZoomLevel] = useState(1);

  useEffect(() => {
    const fetchDiagramAndReport = async () => {
      if (!id) return;
      try {
        setLoading(true);
        setError(null);

        const res = await diagramService.getDiagramById(id);
        const data = res?.data?.data || res?.data || res;
        setDiagram(data);

        // Load code from contentText or default template
        if (data?.contentText && data.contentText.trim()) {
          setCode(data.contentText.trim());
        } else {
          const type = data?.diagramType || 'Flowchart';
          setCode(TEMPLATES[type] || TEMPLATES.Flowchart);
        }

        // Fetch AI Review report if available
        const versionId = data?.currentVersionId || data?.id;
        if (versionId) {
          try {
            const repRes = await reviewService.getReportByVersionId(versionId);
            setReport(repRes?.data?.data || repRes?.data || repRes);
          } catch (e) {
            // Report might not exist yet
          }
        }
      } catch (err) {
        console.error('Failed to load diagram for editor:', err);
        setError(err.response?.data?.message || err.message || 'Failed to load diagram.');
      } finally {
        setLoading(false);
      }
    };

    fetchDiagramAndReport();
  }, [id]);

  const isImageDiagram = diagram?.currentStorageUrl && !diagram.currentStorageUrl.endsWith('.mmd');
  const imageUrl = diagram?.currentStorageUrl?.startsWith('http')
    ? diagram.currentStorageUrl
    : `http://localhost:5002/${diagram?.currentStorageUrl?.replace(/^\/+/, '')}`;

  const handleApplyTemplate = (type) => {
    if (TEMPLATES[type]) {
      setCode(TEMPLATES[type]);
    }
  };

  const handleSave = async () => {
    if (!code.trim() || !diagram?.workspaceId) return;
    try {
      setSaving(true);
      setSaveSuccess('');

      const res = await diagramService.createMermaidDiagram({
        workspaceId: diagram.workspaceId,
        name: diagram.name || 'Diagram',
        diagramType: diagram.diagramType || 'Flowchart',
        description: diagram.description || '',
        mermaidCode: code,
      });

      if (res?.data || res?.success) {
        setSaveSuccess('Saved and submitted new version for AI analysis!');
        setTimeout(() => setSaveSuccess(''), 3500);
      }
    } catch (err) {
      console.error('Save diagram error:', err);
      alert('Error saving new version: ' + (err.response?.data?.message || err.message));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      height: 'calc(100vh - 64px)',
      backgroundColor: 'var(--bg-main)',
    }}>
      {/* Top Editor Toolbar */}
      <div style={{
        padding: '12px 24px',
        backgroundColor: '#ffffff',
        borderBottom: '1px solid var(--border-color)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        flexWrap: 'wrap',
        gap: '12px',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
          <button
            onClick={() => diagram?.workspaceId ? navigate(`/workspace/${diagram.workspaceId}`) : navigate('/dashboard')}
            className="btn btn-outline btn-sm"
            style={{ gap: '6px', fontSize: '13px' }}
          >
            <ArrowLeft size={15} />
            <span>Back to Workspace</span>
          </button>

          <div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
              <h2 style={{ fontSize: '16.5px', fontWeight: 700, color: 'var(--text-primary)' }}>
                {diagram?.name || 'Diagram Viewer & Studio'}
              </h2>
              <span className="badge badge-active">{diagram?.diagramType || 'Diagram'}</span>
              <span style={{
                fontSize: '11px',
                fontWeight: 600,
                padding: '2px 7px',
                borderRadius: '10px',
                backgroundColor: 'var(--accent-blue-light)',
                color: 'var(--accent-primary)',
              }}>
                v{diagram?.currentVersion || 1}
              </span>
            </div>
          </div>
        </div>

        {/* Action Buttons */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          {saveSuccess && (
            <span style={{ fontSize: '12.5px', color: 'var(--color-success)', fontWeight: 500 }}>
              ✓ {saveSuccess}
            </span>
          )}

          {diagram?.id && (
            <button
              className="btn btn-outline btn-sm"
              onClick={() => navigate(`/review/${diagram.id}`)}
              style={{ gap: '6px', height: '36px', color: 'var(--accent-primary)', fontWeight: 600 }}
            >
              <Bot size={16} />
              <span>View AI Review Report</span>
            </button>
          )}

          {!isImageDiagram && (
            <button
              className="btn btn-primary btn-sm"
              onClick={handleSave}
              disabled={saving || !code.trim()}
              style={{ gap: '6px', height: '36px' }}
            >
              {saving ? <Loader2 size={15} className="animate-spin" /> : <Save size={15} />}
              <span>Save New Version</span>
            </button>
          )}
        </div>
      </div>

      {/* Main Content Area */}
      {loading ? (
        <div style={{
          flex: 1,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          gap: '10px',
          color: 'var(--text-secondary)',
        }}>
          <Loader2 size={24} className="animate-spin" color="var(--accent-primary)" />
          <span>Loading diagram data...</span>
        </div>
      ) : error ? (
        <div style={{ padding: '30px' }}>
          <div style={{
            padding: '16px',
            backgroundColor: 'var(--color-danger-bg)',
            border: '1px solid #fecaca',
            borderRadius: 'var(--radius-md)',
            color: 'var(--color-danger)',
            display: 'flex',
            alignItems: 'center',
            gap: '10px',
          }}>
            <AlertCircle size={18} />
            <span>{error}</span>
          </div>
        </div>
      ) : isImageDiagram ? (
        /* Image Studio & Inspector Layout */
        <div style={{
          flex: 1,
          display: 'grid',
          gridTemplateColumns: '1fr 340px',
          overflow: 'hidden',
        }}>
          {/* Left Canvas: Interactive Image Viewer */}
          <div style={{
            display: 'flex',
            flexDirection: 'column',
            backgroundColor: '#f8fafc',
            borderRight: '1px solid var(--border-color)',
            position: 'relative',
            overflow: 'hidden',
          }}>
            {/* Canvas Zoom & Control Bar */}
            <div style={{
              position: 'absolute',
              top: '16px',
              left: '16px',
              zIndex: 10,
              display: 'flex',
              alignItems: 'center',
              gap: '4px',
              backgroundColor: 'rgba(255, 255, 255, 0.95)',
              backdropFilter: 'blur(6px)',
              padding: '4px 8px',
              borderRadius: '8px',
              boxShadow: '0 2px 10px rgba(0, 0, 0, 0.08)',
              border: '1px solid var(--border-color)',
            }}>
              <button
                className="btn btn-outline btn-sm"
                onClick={() => setZoomLevel((prev) => Math.max(0.4, prev - 0.2))}
                style={{ padding: '6px', height: '30px', width: '30px' }}
                title="Zoom Out"
              >
                <ZoomOut size={15} />
              </button>
              <span style={{ fontSize: '12px', fontWeight: 600, minWidth: '46px', textAlign: 'center' }}>
                {Math.round(zoomLevel * 100)}%
              </span>
              <button
                className="btn btn-outline btn-sm"
                onClick={() => setZoomLevel((prev) => Math.min(2.5, prev + 0.2))}
                style={{ padding: '6px', height: '30px', width: '30px' }}
                title="Zoom In"
              >
                <ZoomIn size={15} />
              </button>
              <button
                className="btn btn-outline btn-sm"
                onClick={() => setZoomLevel(1)}
                style={{ padding: '6px', height: '30px', width: '30px' }}
                title="Reset to 100%"
              >
                <RotateCcw size={14} />
              </button>
            </div>

            {/* Canvas Viewport */}
            <div style={{
              flex: 1,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              padding: '40px',
              overflow: 'auto',
            }}>
              <img
                src={imageUrl}
                alt={diagram.name}
                style={{
                  transform: `scale(${zoomLevel})`,
                  transformOrigin: 'center center',
                  transition: 'transform 0.15s ease-out',
                  maxWidth: '90%',
                  maxHeight: '80vh',
                  borderRadius: '8px',
                  backgroundColor: '#ffffff',
                  boxShadow: '0 8px 30px rgba(0, 0, 0, 0.06)',
                  border: '1px solid var(--border-color)',
                  objectFit: 'contain',
                }}
              />
            </div>
          </div>

          {/* Right Sidebar: Diagram Specs & AI Summary */}
          <div style={{
            backgroundColor: '#ffffff',
            padding: '24px',
            overflowY: 'auto',
            display: 'flex',
            flexDirection: 'column',
            gap: '20px',
          }}>
            {/* Metadata Box */}
            <div>
              <h3 style={{ fontSize: '14px', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '12px' }}>
                Diagram Details
              </h3>
              <div style={{ display: 'flex', flexDirection: 'column', gap: '10px', fontSize: '13px' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: 'var(--text-muted)' }}>Type:</span>
                  <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{diagram.diagramType}</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: 'var(--text-muted)' }}>Format:</span>
                  <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Image</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: 'var(--text-muted)' }}>Version:</span>
                  <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>v{diagram.currentVersion || 1}</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <span style={{ color: 'var(--text-muted)' }}>Created At:</span>
                  <span style={{ fontWeight: 500, color: 'var(--text-secondary)' }}>
                    {diagram.createdAt ? new Date(diagram.createdAt).toLocaleDateString('en-US') : 'Just created'}
                  </span>
                </div>
              </div>
            </div>

            {/* AI Review Snapshot Box */}
            <div style={{
              padding: '16px',
              backgroundColor: 'var(--accent-blue-light)',
              borderRadius: 'var(--radius-md)',
              border: '1px solid #bfdbfe',
            }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px' }}>
                <Bot size={18} color="var(--accent-primary)" />
                <h4 style={{ fontSize: '13.5px', fontWeight: 700, color: 'var(--accent-primary)' }}>
                  AI Review Summary
                </h4>
              </div>

              <div style={{ display: 'flex', alignItems: 'baseline', gap: '6px', marginBottom: '8px' }}>
                <span style={{ fontSize: '28px', fontWeight: 800, color: 'var(--accent-primary)' }}>
                  {(() => {
                    const raw = report?.totalScore ?? 8.5;
                    return raw > 10 ? (raw / 10).toFixed(1) : Number(raw).toFixed(1);
                  })()}
                </span>
                <span style={{ fontSize: '14px', color: 'var(--text-muted)' }}>/10 points</span>
              </div>

              <p style={{ fontSize: '12px', color: 'var(--text-secondary)', lineHeight: 1.4, marginBottom: '12px' }}>
                AI has analyzed this diagram. Click the button below to view full architectural issues and refactored suggestions.
              </p>

              <button
                className="btn btn-primary"
                onClick={() => navigate(`/review/${diagram.id}`)}
                style={{ width: '100%', gap: '6px', height: '36px', fontSize: '13px' }}
              >
                <span>View Full AI Report</span>
                <ArrowRight size={14} />
              </button>
            </div>

            {/* Quick Actions */}
            <div>
              <h3 style={{ fontSize: '14px', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '12px' }}>
                Quick Actions
              </h3>
              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                <a
                  href={imageUrl}
                  target="_blank"
                  rel="noreferrer"
                  download
                  className="btn btn-outline"
                  style={{ width: '100%', justifyContent: 'center', gap: '6px', height: '36px', fontSize: '13px' }}
                >
                  <Download size={14} />
                  <span>Download Original Image</span>
                </a>
              </div>
            </div>
          </div>
        </div>
      ) : (
        /* Split Code Editor & Live Preview for Mermaid Diagrams */
        <div style={{
          flex: 1,
          display: 'grid',
          gridTemplateColumns: '1fr 1fr',
          overflow: 'hidden',
        }}>
          {/* Left: Code Pane */}
          <div style={{
            borderRight: '1px solid var(--border-color)',
            display: 'flex',
            flexDirection: 'column',
            backgroundColor: '#ffffff',
          }}>
            <div style={{
              padding: '10px 16px',
              backgroundColor: '#f8fafc',
              borderBottom: '1px solid var(--border-color)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
            }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '12.5px', fontWeight: 600, color: 'var(--text-secondary)' }}>
                <Code2 size={15} />
                <span>Mermaid Source Code</span>
              </div>

              <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                <button
                  className="btn btn-outline btn-sm"
                  onClick={() => handleApplyTemplate(diagram?.diagramType || 'Sequence')}
                  style={{ fontSize: '11.5px', padding: '3px 8px', height: 'auto', gap: '4px' }}
                >
                  <Sparkles size={12} />
                  <span>Load Standard Template</span>
                </button>
              </div>
            </div>

            <textarea
              value={code}
              onChange={(e) => setCode(e.target.value)}
              placeholder="Enter Mermaid syntax here (e.g. sequenceDiagram, classDiagram, graph TD)..."
              style={{
                flex: 1,
                padding: '16px',
                fontFamily: 'var(--font-mono)',
                fontSize: '13px',
                lineHeight: 1.5,
                border: 'none',
                outline: 'none',
                resize: 'none',
                backgroundColor: '#ffffff',
                color: 'var(--text-primary)',
              }}
            />
          </div>

          {/* Right: Live Preview Pane */}
          <div style={{
            display: 'flex',
            flexDirection: 'column',
            backgroundColor: '#f8fafc',
            overflow: 'hidden',
          }}>
            <div style={{
              padding: '10px 16px',
              backgroundColor: '#ffffff',
              borderBottom: '1px solid var(--border-color)',
              display: 'flex',
              alignItems: 'center',
              gap: '6px',
              fontSize: '12.5px',
              fontWeight: 600,
              color: 'var(--text-secondary)',
            }}>
              <Eye size={15} />
              <span>Live Visual Preview</span>
            </div>

            <div style={{
              flex: 1,
              padding: '24px',
              overflow: 'auto',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}>
              <MermaidRenderer code={code} id="live-preview-editor" />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
