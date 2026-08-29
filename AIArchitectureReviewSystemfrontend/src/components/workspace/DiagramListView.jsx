import React, { useState, useMemo } from 'react';
import {
  FileCode,
  Image as ImageIcon,
  Database,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  Trash2,
  Loader2,
  AlertCircle,
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { diagramService } from '../../services/diagramService';

const STATUS_CONFIG = {
  0: { label: 'Pending', color: '#d97706', bg: '#fffbeb', border: '#fde68a' },
  1: { label: 'Processing', color: '#2563eb', bg: '#eff6ff', border: '#bfdbfe' },
  2: { label: 'Completed', color: '#16a34a', bg: '#f0fdf4', border: '#bbf7d0' },
  3: { label: 'Failed', color: '#dc2626', bg: '#fef2f2', border: '#fecaca' },
};

export default function DiagramListView({ diagrams = [], onDelete }) {
  const navigate = useNavigate();

  // Delete State
  const [deletingDiagram, setDeletingDiagram] = useState(null);
  const [isDeleting, setIsDeleting] = useState(false);

  // Pagination State
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);

  const totalItems = diagrams.length;
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));

  // Ensure current page is valid when total changes
  const validCurrentPage = Math.min(currentPage, totalPages);

  const paginatedDiagrams = useMemo(() => {
    const startIndex = (validCurrentPage - 1) * pageSize;
    return diagrams.slice(startIndex, startIndex + pageSize);
  }, [diagrams, validCurrentPage, pageSize]);

  const startIndex = (validCurrentPage - 1) * pageSize + 1;
  const endIndex = Math.min(validCurrentPage * pageSize, totalItems);

  const getDiagramIcon = (diagram) => {
    const isMermaid = diagram.currentStorageUrl?.endsWith('.mmd') || diagram.rawFormat === 'mermaid';
    const type = diagram.diagramType?.toLowerCase() || '';

    if (type.includes('erd') || type.includes('database') || type.includes('db')) {
      return <Database size={18} />;
    }
    if (isMermaid) {
      return <FileCode size={18} />;
    }
    return <ImageIcon size={18} />;
  };

  const getFileName = (diagram) => {
    if (diagram.currentStorageUrl) {
      const parts = diagram.currentStorageUrl.split('/');
      const raw = parts[parts.length - 1];
      // Strip uuid prefix if present (e.g. 1234-uuid_filename.png -> filename.png)
      return raw.replace(/^[a-f0-9-]+_/, '');
    }
    return diagram.diagramType === 'Mermaid' ? `${diagram.name.toLowerCase().replace(/\s+/g, '_')}.mmd` : `${diagram.name.toLowerCase().replace(/\s+/g, '_')}.png`;
  };

  const handlePageChange = (page) => {
    if (page >= 1 && page <= totalPages) {
      setCurrentPage(page);
    }
  };

  const handlePageSizeChange = (e) => {
    const newSize = Number(e.target.value);
    setPageSize(newSize);
    setCurrentPage(1);
  };

  // Generate page numbers to show
  const getPageNumbers = () => {
    const pages = [];
    const maxVisible = 5;
    let start = Math.max(1, validCurrentPage - Math.floor(maxVisible / 2));
    let end = Math.min(totalPages, start + maxVisible - 1);

    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  };

  const handleConfirmDelete = async () => {
    if (!deletingDiagram) return;
    try {
      setIsDeleting(true);
      if (onDelete) {
        await onDelete(deletingDiagram.id);
      } else {
        await diagramService.deleteDiagram(deletingDiagram.id);
      }
      setDeletingDiagram(null);
    } catch (err) {
      console.error('Delete diagram error:', err);
      alert(err.response?.data?.message || err.message || 'Failed to delete diagram.');
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <div className="card" style={{
      padding: 0,
      backgroundColor: '#ffffff',
      borderRadius: 'var(--radius-lg)',
      border: '1px solid var(--border-color)',
      overflow: 'hidden',
      boxShadow: '0 1px 3px rgba(0, 0, 0, 0.04)',
    }}>
      {/* Table Container with Horizontal Scroll */}
      <div style={{ overflowX: 'auto', width: '100%' }}>
        <table style={{
          width: '100%',
          borderCollapse: 'collapse',
          textAlign: 'left',
          fontSize: '13.5px',
        }}>
          <thead>
            <tr style={{
              borderBottom: '1px solid var(--border-color)',
              backgroundColor: '#f8fafc',
              color: 'var(--text-muted)',
              fontSize: '11.5px',
              fontWeight: 700,
              textTransform: 'uppercase',
              letterSpacing: '0.05em',
            }}>
              <th style={{ padding: '14px 20px', width: '35%' }}>Name</th>
              <th style={{ padding: '14px 16px' }}>Type</th>
              <th style={{ padding: '14px 16px' }}>Version</th>
              <th style={{ padding: '14px 16px' }}>Status</th>
              <th style={{ padding: '14px 16px' }}>Last Modified</th>
              <th style={{ padding: '14px 20px', textAlign: 'right' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {paginatedDiagrams.map((diag, index) => {
              const status = STATUS_CONFIG[diag.currentStatus] || {
                label: 'Pending',
                color: '#d97706',
                bg: '#fffbeb',
                border: '#fde68a',
              };

              const isMermaid = diag.currentStorageUrl?.endsWith('.mmd') || diag.rawFormat === 'mermaid';
              const fileName = getFileName(diag);

              return (
                <tr
                  key={diag.id || index}
                  onClick={() => navigate(`/editor/${diag.id}`)}
                  style={{
                    borderBottom: '1px solid var(--border-color)',
                    transition: 'background-color 0.15s ease',
                    cursor: 'pointer',
                  }}
                  onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = 'var(--bg-main)')}
                  onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = 'transparent')}
                >
                  {/* 1. Name & Filename */}
                  <td style={{ padding: '14px 20px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
                      <div style={{
                        width: '38px',
                        height: '38px',
                        borderRadius: '8px',
                        backgroundColor: isMermaid ? 'var(--accent-blue-light)' : '#f3e8ff',
                        color: isMermaid ? 'var(--accent-primary)' : '#7e22ce',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        flexShrink: 0,
                      }}>
                        {getDiagramIcon(diag)}
                      </div>
                      <div style={{ minWidth: 0 }}>
                        <div style={{
                          fontWeight: 600,
                          color: 'var(--text-primary)',
                          fontSize: '14px',
                          marginBottom: '2px',
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap',
                          maxWidth: '280px',
                        }}>
                          {diag.name}
                        </div>
                        <div style={{
                          fontSize: '12px',
                          color: 'var(--text-muted)',
                          fontFamily: 'monospace',
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap',
                          maxWidth: '280px',
                        }}>
                          {fileName}
                        </div>
                      </div>
                    </div>
                  </td>

                  {/* 2. Type */}
                  <td style={{ padding: '14px 16px' }}>
                    <span style={{
                      fontSize: '12px',
                      color: 'var(--text-secondary)',
                      backgroundColor: 'var(--bg-main)',
                      padding: '3px 8px',
                      borderRadius: '4px',
                      border: '1px solid var(--border-color)',
                      fontWeight: 500,
                      display: 'inline-block',
                    }}>
                      {isMermaid ? `Mermaid (${diag.diagramType || 'Code'})` : `Image (${diag.diagramType || 'Image'})`}
                    </span>
                  </td>

                  {/* 3. Version */}
                  <td style={{ padding: '14px 16px', fontWeight: 600, color: 'var(--text-primary)', fontFamily: 'monospace' }}>
                    v{diag.currentVersion ? Number(diag.currentVersion).toFixed(1) : '1.0'}
                  </td>

                  {/* 4. Status Badge */}
                  <td style={{ padding: '14px 16px' }}>
                    <span style={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      gap: '6px',
                      padding: '3px 10px',
                      borderRadius: '12px',
                      fontSize: '11.5px',
                      fontWeight: 600,
                      color: status.color,
                      backgroundColor: status.bg,
                      border: `1px solid ${status.border}`,
                    }}>
                      <span style={{
                        width: '6px',
                        height: '6px',
                        borderRadius: '50%',
                        backgroundColor: status.color,
                        display: 'inline-block',
                      }} />
                      {status.label}
                    </span>
                  </td>

                  {/* 5. Last Modified */}
                  <td style={{ padding: '14px 16px', color: 'var(--text-secondary)', fontSize: '13px', whiteSpace: 'nowrap' }}>
                    {diag.createdAt ? new Date(diag.createdAt).toLocaleDateString('en-US') : 'Just created'}
                  </td>

                  {/* 6. Actions */}
                  <td style={{ padding: '14px 20px', textAlign: 'right', whiteSpace: 'nowrap' }}>
                    <div onClick={(e) => e.stopPropagation()} style={{ display: 'inline-flex', alignItems: 'center', gap: '8px' }}>
                      <button
                        className="btn btn-primary btn-sm"
                        onClick={() => navigate(`/review/${diag.id}`)}
                        title="View AI Architecture Review Report"
                        style={{
                          padding: '6px 16px',
                          fontSize: '13px',
                          height: '34px',
                          fontWeight: 500,
                          whiteSpace: 'nowrap',
                          minWidth: '115px',
                        }}
                      >
                        View Report
                      </button>

                      <button
                        className="btn btn-outline btn-sm"
                        onClick={() => setDeletingDiagram(diag)}
                        title="Delete Diagram"
                        style={{
                          padding: '6px',
                          height: '34px',
                          width: '34px',
                          borderRadius: 'var(--radius-md)',
                          color: '#ef4444',
                          borderColor: '#fee2e2',
                          display: 'inline-flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          backgroundColor: '#ffffff',
                          cursor: 'pointer',
                          transition: 'all 0.15s ease',
                        }}
                        onMouseEnter={(e) => {
                          e.currentTarget.style.backgroundColor = '#fef2f2';
                          e.currentTarget.style.borderColor = '#fca5a5';
                        }}
                        onMouseLeave={(e) => {
                          e.currentTarget.style.backgroundColor = '#ffffff';
                          e.currentTarget.style.borderColor = '#fee2e2';
                        }}
                      >
                        <Trash2 size={15} />
                      </button>
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {/* Pagination Footer */}
      {totalItems > 0 && (
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          padding: '12px 20px',
          borderTop: '1px solid var(--border-color)',
          backgroundColor: '#ffffff',
          flexWrap: 'wrap',
          gap: '12px',
        }}>
          {/* Items range description & Page size selector */}
          <div style={{ display: 'flex', alignItems: 'center', gap: '16px', fontSize: '13px', color: 'var(--text-secondary)' }}>
            <span>
              Showing <strong style={{ color: 'var(--text-primary)' }}>{startIndex}</strong> to{' '}
              <strong style={{ color: 'var(--text-primary)' }}>{endIndex}</strong> of{' '}
              <strong style={{ color: 'var(--text-primary)' }}>{totalItems}</strong> diagrams
            </span>

            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <span style={{ fontSize: '12px', color: 'var(--text-muted)' }}>Per page:</span>
              <select
                value={pageSize}
                onChange={handlePageSizeChange}
                style={{
                  padding: '3px 8px',
                  borderRadius: 'var(--radius-sm)',
                  border: '1px solid var(--border-color)',
                  backgroundColor: '#ffffff',
                  fontSize: '12.5px',
                  color: 'var(--text-primary)',
                  cursor: 'pointer',
                  outline: 'none',
                }}
              >
                <option value={5}>5</option>
                <option value={10}>10</option>
                <option value={20}>20</option>
                <option value={50}>50</option>
              </select>
            </div>
          </div>

          {/* Navigation Controls */}
          <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
            {/* First Page */}
            <button
              onClick={() => handlePageChange(1)}
              disabled={validCurrentPage === 1}
              title="First Page"
              style={{
                width: '32px',
                height: '32px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                borderRadius: 'var(--radius-sm)',
                border: '1px solid var(--border-color)',
                backgroundColor: '#ffffff',
                color: validCurrentPage === 1 ? 'var(--text-muted)' : 'var(--text-primary)',
                cursor: validCurrentPage === 1 ? 'not-allowed' : 'pointer',
                opacity: validCurrentPage === 1 ? 0.5 : 1,
              }}
            >
              <ChevronsLeft size={15} />
            </button>

            {/* Prev Page */}
            <button
              onClick={() => handlePageChange(validCurrentPage - 1)}
              disabled={validCurrentPage === 1}
              title="Previous Page"
              style={{
                width: '32px',
                height: '32px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                borderRadius: 'var(--radius-sm)',
                border: '1px solid var(--border-color)',
                backgroundColor: '#ffffff',
                color: validCurrentPage === 1 ? 'var(--text-muted)' : 'var(--text-primary)',
                cursor: validCurrentPage === 1 ? 'not-allowed' : 'pointer',
                opacity: validCurrentPage === 1 ? 0.5 : 1,
              }}
            >
              <ChevronLeft size={15} />
            </button>

            {/* Page Number Buttons */}
            {getPageNumbers().map((pageNum) => {
              const isActive = pageNum === validCurrentPage;
              return (
                <button
                  key={pageNum}
                  onClick={() => handlePageChange(pageNum)}
                  style={{
                    minWidth: '32px',
                    height: '32px',
                    padding: '0 6px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    borderRadius: 'var(--radius-sm)',
                    border: isActive ? '1px solid var(--accent-primary)' : '1px solid var(--border-color)',
                    backgroundColor: isActive ? 'var(--accent-primary)' : '#ffffff',
                    color: isActive ? '#ffffff' : 'var(--text-primary)',
                    fontWeight: isActive ? 700 : 500,
                    fontSize: '13px',
                    cursor: 'pointer',
                    transition: 'all 0.15s ease',
                  }}
                >
                  {pageNum}
                </button>
              );
            })}

            {/* Next Page */}
            <button
              onClick={() => handlePageChange(validCurrentPage + 1)}
              disabled={validCurrentPage === totalPages}
              title="Next Page"
              style={{
                width: '32px',
                height: '32px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                borderRadius: 'var(--radius-sm)',
                border: '1px solid var(--border-color)',
                backgroundColor: '#ffffff',
                color: validCurrentPage === totalPages ? 'var(--text-muted)' : 'var(--text-primary)',
                cursor: validCurrentPage === totalPages ? 'not-allowed' : 'pointer',
                opacity: validCurrentPage === totalPages ? 0.5 : 1,
              }}
            >
              <ChevronRight size={15} />
            </button>

            {/* Last Page */}
            <button
              onClick={() => handlePageChange(totalPages)}
              disabled={validCurrentPage === totalPages}
              title="Last Page"
              style={{
                width: '32px',
                height: '32px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                borderRadius: 'var(--radius-sm)',
                border: '1px solid var(--border-color)',
                backgroundColor: '#ffffff',
                color: validCurrentPage === totalPages ? 'var(--text-muted)' : 'var(--text-primary)',
                cursor: validCurrentPage === totalPages ? 'not-allowed' : 'pointer',
                opacity: validCurrentPage === totalPages ? 0.5 : 1,
              }}
            >
              <ChevronsRight size={15} />
            </button>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {deletingDiagram && (
        <div
          className="modal-backdrop"
          onClick={() => !isDeleting && setDeletingDiagram(null)}
          style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(15, 23, 42, 0.6)',
            backdropFilter: 'blur(4px)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            zIndex: 9999,
          }}
        >
          <div
            className="modal-content card"
            style={{
              maxWidth: '440px',
              width: '90%',
              padding: '24px',
              backgroundColor: '#ffffff',
              borderRadius: 'var(--radius-lg)',
              boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
            }}
            onClick={(e) => e.stopPropagation()}
          >
            <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '16px' }}>
              <div
                style={{
                  width: '42px',
                  height: '42px',
                  borderRadius: '50%',
                  backgroundColor: '#fee2e2',
                  color: '#dc2626',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  flexShrink: 0,
                }}
              >
                <Trash2 size={20} />
              </div>
              <div>
                <h3 style={{ fontSize: '16px', fontWeight: 600, color: 'var(--text-primary)', margin: 0 }}>
                  Delete Diagram
                </h3>
                <p style={{ fontSize: '12px', color: 'var(--text-secondary)', margin: '2px 0 0' }}>
                  This action cannot be undone.
                </p>
              </div>
            </div>

            <p style={{ fontSize: '13.5px', color: 'var(--text-secondary)', lineHeight: 1.5, marginBottom: '20px' }}>
              Are you sure you want to delete <strong style={{ color: 'var(--text-primary)' }}>"{deletingDiagram.name}"</strong>? All versions, image files, and AI review history will be permanently deleted.
            </p>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px' }}>
              <button
                type="button"
                className="btn btn-outline"
                onClick={() => setDeletingDiagram(null)}
                disabled={isDeleting}
              >
                Cancel
              </button>
              <button
                type="button"
                className="btn"
                onClick={handleConfirmDelete}
                disabled={isDeleting}
                style={{
                  minWidth: '120px',
                  backgroundColor: '#dc2626',
                  borderColor: '#dc2626',
                  color: '#ffffff',
                  gap: '6px',
                }}
              >
                {isDeleting ? (
                  <>
                    <Loader2 size={15} className="animate-spin" />
                    <span>Deleting...</span>
                  </>
                ) : (
                  <>
                    <Trash2 size={15} />
                    <span>Delete</span>
                  </>
                )}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
