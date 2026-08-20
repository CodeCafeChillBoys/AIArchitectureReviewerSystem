import React, { useState, useMemo } from 'react';
import {
  FileCode,
  Image as ImageIcon,
  Database,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';

const STATUS_CONFIG = {
  0: { label: 'Pending', color: '#d97706', bg: '#fffbeb', border: '#fde68a' },
  1: { label: 'Processing', color: '#2563eb', bg: '#eff6ff', border: '#bfdbfe' },
  2: { label: 'Completed', color: '#16a34a', bg: '#f0fdf4', border: '#bbf7d0' },
  3: { label: 'Failed', color: '#dc2626', bg: '#fef2f2', border: '#fecaca' },
};

export default function DiagramListView({ diagrams = [] }) {
  const navigate = useNavigate();

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

  return (
    <div className="card" style={{
      padding: 0,
      backgroundColor: '#ffffff',
      borderRadius: 'var(--radius-lg)',
      border: '1px solid var(--border-color)',
      overflow: 'hidden',
      boxShadow: '0 1px 3px rgba(0, 0, 0, 0.04)',
    }}>
      <div style={{ overflowX: 'auto' }}>
        <table style={{
          width: '100%',
          borderCollapse: 'collapse',
          textAlign: 'left',
          fontSize: '13.5px',
        }}>
          <thead>
            <tr style={{
              backgroundColor: '#f8fafc',
              borderBottom: '1px solid var(--border-color)',
              color: 'var(--text-muted)',
              fontSize: '12px',
              fontWeight: 600,
              textTransform: 'uppercase',
              letterSpacing: '0.04em',
            }}>
              <th style={{ padding: '14px 20px', width: '38%' }}>Name</th>
              <th style={{ padding: '14px 16px', width: '15%' }}>Type</th>
              <th style={{ padding: '14px 16px', width: '10%' }}>Version</th>
              <th style={{ padding: '14px 16px', width: '15%' }}>Status</th>
              <th style={{ padding: '14px 16px', width: '12%' }}>Last Modified</th>
              <th style={{ padding: '14px 20px', width: '10%', textAlign: 'right' }}>Actions</th>
            </tr>
          </thead>

          <tbody>
            {paginatedDiagrams.map((diag, index) => {
              const isMermaid = diag.currentStorageUrl?.endsWith('.mmd') || diag.rawFormat === 'mermaid';
              const status = STATUS_CONFIG[diag.currentStatus] || STATUS_CONFIG[0];
              const fileName = getFileName(diag);

              return (
                <tr
                  key={diag.id || index}
                  onClick={() => navigate(`/editor/${diag.id}`)}
                  style={{
                    borderBottom: index === paginatedDiagrams.length - 1 ? 'none' : '1px solid var(--border-color)',
                    cursor: 'pointer',
                    transition: 'all 0.15s ease',
                    backgroundColor: '#ffffff',
                  }}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.backgroundColor = '#f8fafc';
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.backgroundColor = '#ffffff';
                  }}
                >
                  {/* 1. Name & File Subtitle */}
                  <td style={{ padding: '14px 20px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
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

                      <div style={{ overflow: 'hidden' }}>
                        <div style={{
                          fontWeight: 600,
                          color: 'var(--text-primary)',
                          fontSize: '14px',
                          marginBottom: '2px',
                        }}>
                          {diag.name}
                        </div>
                        <div style={{
                          fontSize: '12px',
                          color: 'var(--text-muted)',
                          fontFamily: 'var(--font-mono)',
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap',
                        }}>
                          {fileName}
                        </div>
                      </div>
                    </div>
                  </td>

                  {/* 2. Type Badge */}
                  <td style={{ padding: '14px 16px', whiteSpace: 'nowrap' }}>
                    <span style={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      padding: '3px 10px',
                      borderRadius: '6px',
                      backgroundColor: '#f1f5f9',
                      border: '1px solid #e2e8f0',
                      color: 'var(--text-secondary)',
                      fontSize: '12px',
                      fontWeight: 500,
                      fontFamily: isMermaid ? 'var(--font-mono)' : 'inherit',
                    }}>
                      {isMermaid ? 'Mermaid' : 'Image'}
                      {diag.diagramType && ` (${diag.diagramType})`}
                    </span>
                  </td>

                  {/* 3. Version */}
                  <td style={{ padding: '14px 16px', whiteSpace: 'nowrap' }}>
                    <span style={{
                      fontSize: '13px',
                      fontWeight: 600,
                      color: 'var(--text-primary)',
                      fontFamily: 'var(--font-mono)',
                    }}>
                      v{diag.currentVersion || 1}.0
                    </span>
                  </td>

                  {/* 4. AI Status */}
                  <td style={{ padding: '14px 16px', whiteSpace: 'nowrap' }}>
                    <span style={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      gap: '5px',
                      padding: '3px 8px',
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
                    <div onClick={(e) => e.stopPropagation()} style={{ display: 'inline-block' }}>
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
    </div>
  );
}
