import React, { useState, useRef } from 'react';
import { X, Upload, Loader2, AlertCircle, Image as ImageIcon, FileText } from 'lucide-react';

const DIAGRAM_TYPES = [
  { value: 'Flowchart', label: 'Flowchart' },
  { value: 'Sequence', label: 'Sequence Diagram' },
  { value: 'Class', label: 'Class Diagram' },
  { value: 'ERD', label: 'ERD (Database Entity Relationship)' },
  { value: 'Architecture', label: 'System Architecture' },
  { value: 'Other', label: 'Other' },
];

export default function UploadDiagramModal({
  isOpen,
  onClose,
  onSubmit,
  loading = false,
  error = '',
}) {
  const [name, setName] = useState('');
  const [diagramType, setDiagramType] = useState('Architecture');
  const [customType, setCustomType] = useState('');
  const [description, setDescription] = useState('');
  const [selectedFile, setSelectedFile] = useState(null);
  const [previewUrl, setPreviewUrl] = useState('');
  const [validationError, setValidationError] = useState('');
  const fileInputRef = useRef(null);

  if (!isOpen) return null;

  const handleFileChange = (e) => {
    const file = e.target.files?.[0];
    if (file) {
      setSelectedFile(file);
      if (!name) {
        const cleanName = file.name.replace(/\.[^/.]+$/, '');
        setName(cleanName);
      }
      if (file.type.startsWith('image/')) {
        setPreviewUrl(URL.createObjectURL(file));
      } else {
        setPreviewUrl('');
      }
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
    if (!selectedFile) {
      setValidationError('Please select a diagram file to upload.');
      return;
    }

    const finalDiagramType = diagramType === 'Other' ? customType.trim() : diagramType;

    setValidationError('');
    onSubmit({
      name: name.trim(),
      diagramType: finalDiagramType,
      description: description.trim(),
      imageFile: selectedFile,
    });
  };

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div
        className="modal-content card"
        style={{ maxWidth: '580px', width: '90%', padding: '24px' }}
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
              <Upload size={20} />
            </div>
            <div>
              <h2 style={{ fontSize: '17px', fontWeight: 600, color: 'var(--text-primary)' }}>
                Upload Architecture Diagram
              </h2>
              <p style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>
                Processed & reviewed by AI
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

        {/* Error Alert */}
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
              placeholder="e.g. High Level Microservices Architecture"
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
                  onChange={(e) => setDiagramType(e.target.value)}
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

          {/* File Upload Box */}
          <div>
            <label className="form-label" style={{ fontSize: '13px', fontWeight: 500, marginBottom: '6px', display: 'block' }}>
              Diagram File (.png, .jpg, .jpeg) <span style={{ color: 'var(--color-danger)' }}>*</span>
            </label>
            <input
              type="file"
              ref={fileInputRef}
              onChange={handleFileChange}
              accept="image/png, image/jpeg, image/jpg"
              style={{ display: 'none' }}
            />

            <div
              onClick={() => fileInputRef.current?.click()}
              style={{
                border: '2px dashed var(--border-color)',
                borderRadius: 'var(--radius-md)',
                padding: '24px 16px',
                textAlign: 'center',
                cursor: 'pointer',
                backgroundColor: 'var(--bg-main)',
                transition: 'all 0.2s ease',
              }}
              onDragOver={(e) => e.preventDefault()}
              onDrop={(e) => {
                e.preventDefault();
                const file = e.dataTransfer.files?.[0];
                if (file) {
                  setSelectedFile(file);
                  if (!name) setName(file.name.replace(/\.[^/.]+$/, ''));
                  if (file.type.startsWith('image/')) {
                    setPreviewUrl(URL.createObjectURL(file));
                  }
                }
              }}
            >
              {previewUrl ? (
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px' }}>
                  <img
                    src={previewUrl}
                    alt="Preview"
                    style={{
                      maxHeight: '140px',
                      maxWidth: '100%',
                      borderRadius: '6px',
                      objectFit: 'contain',
                      border: '1px solid var(--border-color)',
                    }}
                  />
                  <span style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>
                    {selectedFile?.name} ({(selectedFile?.size / 1024).toFixed(1)} KB) - Click to replace image
                  </span>
                </div>
              ) : selectedFile ? (
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '8px' }}>
                  <FileText size={24} color="var(--accent-primary)" />
                  <span style={{ fontSize: '13px', fontWeight: 500, color: 'var(--text-primary)' }}>
                    {selectedFile.name}
                  </span>
                </div>
              ) : (
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '6px' }}>
                  <div style={{
                    width: '40px',
                    height: '40px',
                    borderRadius: '50%',
                    backgroundColor: 'var(--accent-blue-light)',
                    color: 'var(--accent-primary)',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                  }}>
                    <ImageIcon size={20} />
                  </div>
                  <p style={{ fontSize: '13px', fontWeight: 500, color: 'var(--text-primary)' }}>
                    Click to browse or drag & drop diagram file here
                  </p>
                  <p style={{ fontSize: '12px', color: 'var(--text-muted)' }}>
                    Supported formats: PNG, JPG, JPEG
                  </p>
                </div>
              )}
            </div>
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
                  <span>Uploading...</span>
                </>
              ) : (
                <>
                  <Upload size={16} />
                  <span>Upload Diagram</span>
                </>
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
