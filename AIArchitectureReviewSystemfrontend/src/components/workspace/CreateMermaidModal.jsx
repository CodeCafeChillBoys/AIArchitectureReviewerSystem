import React, { useState } from 'react';
import { X, Code2, Loader2, AlertCircle, Sparkles } from 'lucide-react';

const DIAGRAM_TYPES = [
  { value: 'Flowchart', label: 'Flowchart' },
  { value: 'Sequence', label: 'Sequence Diagram' },
  { value: 'Class', label: 'Class Diagram' },
  { value: 'ERD', label: 'ERD (Database Entity Relationship)' },
  { value: 'Architecture', label: 'System Architecture' },
  { value: 'State', label: 'State Diagram' },
  { value: 'UseCase', label: 'Use Case Diagram' },
  { value: 'Other', label: 'Other' },
];

const MERMAID_TEMPLATES = {
  Flowchart: `graph TD
    A[Start] --> B{Check Condition}
    B -- Valid --> C[Process Request]
    B -- Invalid --> D[Return Error]
    C --> E[Complete]`,
  Sequence: `sequenceDiagram
    autonumber
    actor Client as User
    participant API as API Gateway
    participant Auth as Auth Service
    participant DB as Database

    Client->>API: POST /api/auth/login
    API->>Auth: Validate Credentials
    Auth->>DB: Query User Record
    DB-->>Auth: Return User Data
    Auth-->>API: 200 OK + JWT Token
    API-->>Client: Login Successful`,
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
    DIAGRAMS ||--o{ DIAGRAM_VERSIONS : tracks

    USERS {
        uuid id PK
        string email
        string full_name
    }
    WORKSPACES {
        uuid id PK
        uuid user_id FK
        string name
    }
    DIAGRAMS {
        uuid id PK
        uuid workspace_id FK
        string name
        string diagram_type
    }`,
  Architecture: `graph TB
    subgraph Frontend [Client Layer]
        Web[Web React App]
        Mobile[Mobile App]
    end
    subgraph Gateway [API Gateway]
        Ocelot[API Gateway / Reverse Proxy]
    end
    subgraph Services [Microservices]
        AuthSvc[Auth Service]
        DiagramSvc[Diagram Service]
        AISvc[AI Reviewer Service]
    end
    Web --> Ocelot
    Mobile --> Ocelot
    Ocelot --> AuthSvc
    Ocelot --> DiagramSvc
    Ocelot --> AISvc`,
};

export default function CreateMermaidModal({
  isOpen,
  onClose,
  onSubmit,
  loading = false,
  error = '',
}) {
  const [name, setName] = useState('');
  const [diagramType, setDiagramType] = useState('Flowchart');
  const [customType, setCustomType] = useState('');
  const [description, setDescription] = useState('');
  const [mermaidCode, setMermaidCode] = useState(MERMAID_TEMPLATES.Flowchart);
  const [validationError, setValidationError] = useState('');

  if (!isOpen) return null;

  const handleTypeChange = (type) => {
    setDiagramType(type);
    if (MERMAID_TEMPLATES[type]) {
      setMermaidCode(MERMAID_TEMPLATES[type]);
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!name.trim()) {
      setValidationError('Please enter diagram name.');
      return;
    }
    if (diagramType === 'Other' && !customType.trim()) {
      setValidationError('Please enter the custom diagram type.');
      return;
    }
    if (!mermaidCode.trim()) {
      setValidationError('Please enter Mermaid code.');
      return;
    }

    const finalDiagramType = diagramType === 'Other' ? customType.trim() : diagramType;

    setValidationError('');
    onSubmit({
      name: name.trim(),
      diagramType: finalDiagramType,
      description: description.trim(),
      mermaidCode: mermaidCode.trim(),
    });
  };

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div
        className="modal-content card"
        style={{ maxWidth: '640px', width: '90%', padding: '24px' }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          marginBottom: '20px',
          paddingBottom: '12px',
          borderBottom: '1px solid var(--border-color)',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <div style={{
              width: '36px',
              height: '36px',
              borderRadius: '8px',
              backgroundColor: 'var(--accent-blue-light)',
              color: 'var(--accent-primary)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}>
              <Code2 size={20} />
            </div>
            <div>
              <h2 style={{ fontSize: '17px', fontWeight: 600, color: 'var(--text-primary)' }}>
                Create New Mermaid Diagram
              </h2>
              <p style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>
                Directly parsed & reviewed by AI
              </p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="btn btn-outline btn-sm"
            style={{ padding: '6px', borderRadius: '50%' }}
          >
            <X size={16} />
          </button>
        </div>

        {/* Error alert */}
        {(error || validationError) && (
          <div style={{
            display: 'flex',
            alignItems: 'center',
            gap: '8px',
            padding: '10px 14px',
            backgroundColor: 'var(--color-danger-bg)',
            border: '1px solid #fecaca',
            borderRadius: 'var(--radius-md)',
            color: 'var(--color-danger)',
            fontSize: '13px',
            marginBottom: '16px',
          }}>
            <AlertCircle size={16} />
            <span>{error || validationError}</span>
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
          <div>
            <label className="form-label" style={{ fontSize: '13px', fontWeight: 500, marginBottom: '6px', display: 'block' }}>
              Diagram Name <span style={{ color: 'var(--color-danger)' }}>*</span>
            </label>
            <input
              type="text"
              className="input-text"
              placeholder="e.g. Core Payment Service Flow"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={loading}
              autoFocus
            />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: diagramType === 'Other' ? '1fr' : '1fr 1fr', gap: '14px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: diagramType === 'Other' ? '1fr 1fr' : '1fr', gap: '14px' }}>
              <div>
                <label className="form-label" style={{ fontSize: '13px', fontWeight: 500, marginBottom: '6px', display: 'block' }}>
                  Diagram Type <span style={{ color: 'var(--color-danger)' }}>*</span>
                </label>
                <select
                  className="input-text"
                  value={diagramType}
                  onChange={(e) => handleTypeChange(e.target.value)}
                  disabled={loading}
                  style={{ cursor: 'pointer' }}
                >
                  {DIAGRAM_TYPES.map((t) => (
                    <option key={t.value} value={t.value}>
                      {t.label}
                    </option>
                  ))}
                </select>
              </div>

              {diagramType === 'Other' && (
                <div>
                  <label className="form-label" style={{ fontSize: '13px', fontWeight: 500, marginBottom: '6px', display: 'block' }}>
                    Custom Diagram Type <span style={{ color: 'var(--color-danger)' }}>*</span>
                  </label>
                  <input
                    type="text"
                    className="input-text"
                    placeholder="e.g. C4 Model, Network Topology..."
                    value={customType}
                    onChange={(e) => setCustomType(e.target.value)}
                    disabled={loading}
                    autoFocus
                  />
                </div>
              )}
            </div>

            <div>
              <label className="form-label" style={{ fontSize: '13px', fontWeight: 500, marginBottom: '6px', display: 'block' }}>
                Description (Optional)
              </label>
              <input
                type="text"
                className="input-text"
                placeholder="Optional notes..."
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                disabled={loading}
              />
            </div>
          </div>

          <div>
            <div style={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              marginBottom: '6px',
            }}>
              <label className="form-label" style={{ fontSize: '13px', fontWeight: 500, display: 'block' }}>
                Mermaid.js Source Code <span style={{ color: 'var(--color-danger)' }}>*</span>
              </label>
              <button
                type="button"
                className="btn btn-outline btn-sm"
                onClick={() => handleTypeChange(diagramType)}
                style={{ fontSize: '11px', padding: '2px 8px', height: 'auto', gap: '4px' }}
              >
                <Sparkles size={12} />
                <span>Load Template</span>
              </button>
            </div>

            <textarea
              className="input-text"
              rows={8}
              value={mermaidCode}
              onChange={(e) => setMermaidCode(e.target.value)}
              disabled={loading}
              placeholder="Enter Mermaid syntax here..."
              style={{
                fontFamily: 'var(--font-mono)',
                fontSize: '12.5px',
                lineHeight: 1.45,
                whiteSpace: 'pre',
              }}
            />
          </div>

          {/* Footer actions */}
          <div style={{
            display: 'flex',
            justifyContent: 'flex-end',
            gap: '10px',
            marginTop: '8px',
            paddingTop: '14px',
            borderTop: '1px solid var(--border-color)',
          }}>
            <button
              type="button"
              className="btn btn-outline"
              onClick={onClose}
              disabled={loading}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
              style={{ minWidth: '130px' }}
            >
              {loading ? (
                <>
                  <Loader2 size={16} className="animate-spin" />
                  <span>Creating...</span>
                </>
              ) : (
                <>
                  <Code2 size={16} />
                  <span>Create Diagram</span>
                </>
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
