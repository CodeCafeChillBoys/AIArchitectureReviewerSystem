import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, FolderKanban, FileCode2, X, Loader2, ArrowRight } from 'lucide-react';
import { workspaceService } from '../../services/workspaceService';
import { diagramService } from '../../services/diagramService';
import { authService } from '../../services/authService';

export default function GlobalSearchBar() {
  const navigate = useNavigate();
  const [query, setQuery] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [workspaces, setWorkspaces] = useState([]);
  const [diagrams, setDiagrams] = useState([]);
  const containerRef = useRef(null);

  // Close when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (containerRef.current && !containerRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };
    const handleKeyDown = (event) => {
      if (event.key === 'Escape') {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    document.addEventListener('keydown', handleKeyDown);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
      document.removeEventListener('keydown', handleKeyDown);
    };
  }, []);

  // Fetch workspaces & their diagrams when query changes or focused
  useEffect(() => {
    if (!query.trim()) {
      setWorkspaces([]);
      setDiagrams([]);
      return;
    }

    const timer = setTimeout(async () => {
      const userId = authService.getUserId();
      if (!userId) return;

      setLoading(true);
      try {
        // 1. Fetch Workspaces
        const wsRes = await workspaceService.getUserWorkspaces(userId, 1, 50);
        const wsItems = wsRes?.data?.items || wsRes?.data || wsRes || [];
        const allWs = Array.isArray(wsItems) ? wsItems : [];

        // Filter matched workspaces
        const matchedWs = allWs.filter((w) =>
          (w.name || '').toLowerCase().includes(query.toLowerCase())
        );
        setWorkspaces(matchedWs.slice(0, 5));

        // 2. Fetch Diagrams from all active workspaces
        const diagPromises = allWs.slice(0, 10).map((w) =>
          diagramService.getWorkspaceDiagrams(w.id, 1, 30).catch(() => null)
        );
        const diagResults = await Promise.all(diagPromises);
        let allDiags = [];
        diagResults.forEach((res, index) => {
          const items = res?.data?.items || res?.data || res || [];
          if (Array.isArray(items)) {
            const wsName = allWs[index]?.name || '';
            items.forEach((d) => {
              allDiags.push({ ...d, workspaceName: wsName });
            });
          }
        });

        // Filter matched diagrams
        const matchedDiags = allDiags.filter(
          (d) =>
            (d.name || '').toLowerCase().includes(query.toLowerCase()) ||
            (d.diagramType || '').toLowerCase().includes(query.toLowerCase()) ||
            (d.description || '').toLowerCase().includes(query.toLowerCase())
        );
        setDiagrams(matchedDiags.slice(0, 6));
      } catch (err) {
        console.error('Error during global search:', err);
      } finally {
        setLoading(false);
      }
    }, 200);

    return () => clearTimeout(timer);
  }, [query]);

  const handleSelectWorkspace = (wsId) => {
    setIsOpen(false);
    setQuery('');
    navigate(`/workspace/${wsId}`);
  };

  const handleSelectDiagram = (diagId) => {
    setIsOpen(false);
    setQuery('');
    navigate(`/editor/${diagId}`);
  };

  const hasResults = workspaces.length > 0 || diagrams.length > 0;

  return (
    <div style={{ position: 'relative', width: '290px' }} ref={containerRef}>
      {/* Search Input Box */}
      <div style={{ position: 'relative' }}>
        <Search
          size={15}
          style={{
            position: 'absolute',
            left: '11px',
            top: '50%',
            transform: 'translateY(-50%)',
            color: isOpen ? 'var(--accent-primary)' : 'var(--text-muted)',
            pointerEvents: 'none',
            transition: 'color 0.15s ease',
          }}
        />
        <input
          type="text"
          placeholder="Search workspaces, diagrams..."
          className="input-text"
          value={query}
          onFocus={() => setIsOpen(true)}
          onChange={(e) => {
            setQuery(e.target.value);
            setIsOpen(true);
          }}
          style={{
            paddingLeft: '34px',
            paddingRight: query ? '30px' : '12px',
            height: '36px',
            fontSize: '12.5px',
            borderRadius: '8px',
            borderColor: isOpen ? 'var(--accent-primary)' : 'var(--border-color)',
            boxShadow: isOpen ? '0 0 0 3px rgba(37, 99, 235, 0.12)' : 'none',
            transition: 'all 0.15s ease',
          }}
        />

        {query && (
          <button
            onClick={() => {
              setQuery('');
              setWorkspaces([]);
              setDiagrams([]);
            }}
            style={{
              position: 'absolute',
              right: '8px',
              top: '50%',
              transform: 'translateY(-50%)',
              background: 'none',
              border: 'none',
              cursor: 'pointer',
              color: 'var(--text-muted)',
              display: 'flex',
              alignItems: 'center',
              padding: '2px',
            }}
          >
            <X size={14} />
          </button>
        )}
      </div>

      {/* Instant Search Dropdown Results */}
      {isOpen && query.trim() && (
        <div
          style={{
            position: 'absolute',
            top: 'calc(100% + 6px)',
            left: 0,
            right: 0,
            minWidth: '340px',
            backgroundColor: '#ffffff',
            borderRadius: '10px',
            border: '1px solid var(--border-color)',
            boxShadow: '0 12px 30px -4px rgba(0, 0, 0, 0.12), 0 4px 10px -2px rgba(0, 0, 0, 0.05)',
            zIndex: 999,
            overflow: 'hidden',
            animation: 'fadeIn 0.15s ease-out',
          }}
        >
          {loading ? (
            <div style={{ padding: '20px', textAlign: 'center', color: 'var(--text-muted)', fontSize: '12.5px' }}>
              <Loader2 size={18} className="animate-spin" style={{ margin: '0 auto 6px', color: 'var(--accent-primary)' }} />
              <div>Đang tìm kiếm...</div>
            </div>
          ) : !hasResults ? (
            <div style={{ padding: '20px', textAlign: 'center', color: 'var(--text-muted)', fontSize: '12.5px' }}>
              Không tìm thấy kết quả cho "<strong>{query}</strong>"
            </div>
          ) : (
            <div style={{ maxHeight: '380px', overflowY: 'auto', padding: '6px' }}>
              {/* Section 1: Workspaces */}
              {workspaces.length > 0 && (
                <div style={{ marginBottom: '8px' }}>
                  <div style={{
                    fontSize: '11px',
                    fontWeight: 700,
                    textTransform: 'uppercase',
                    color: 'var(--text-muted)',
                    padding: '6px 10px 4px',
                    letterSpacing: '0.04em',
                  }}>
                    Workspaces ({workspaces.length})
                  </div>
                  {workspaces.map((ws) => (
                    <div
                      key={ws.id}
                      onClick={() => handleSelectWorkspace(ws.id)}
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                        padding: '8px 10px',
                        borderRadius: '6px',
                        cursor: 'pointer',
                        transition: 'background-color 0.15s ease',
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = '#f1f5f9';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = 'transparent';
                      }}
                    >
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px', overflow: 'hidden' }}>
                        <div style={{
                          width: '26px',
                          height: '26px',
                          borderRadius: '6px',
                          backgroundColor: '#eff6ff',
                          color: '#2563eb',
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          flexShrink: 0,
                        }}>
                          <FolderKanban size={14} />
                        </div>
                        <span style={{ fontSize: '13px', fontWeight: 500, color: 'var(--text-primary)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                          {ws.name}
                        </span>
                      </div>
                      <ArrowRight size={13} color="var(--text-muted)" style={{ flexShrink: 0 }} />
                    </div>
                  ))}
                </div>
              )}

              {/* Section 2: Diagrams */}
              {diagrams.length > 0 && (
                <div>
                  <div style={{
                    fontSize: '11px',
                    fontWeight: 700,
                    textTransform: 'uppercase',
                    color: 'var(--text-muted)',
                    padding: '6px 10px 4px',
                    letterSpacing: '0.04em',
                    borderTop: workspaces.length > 0 ? '1px solid #f1f5f9' : 'none',
                  }}>
                    Diagrams ({diagrams.length})
                  </div>
                  {diagrams.map((d) => (
                    <div
                      key={d.id}
                      onClick={() => handleSelectDiagram(d.id)}
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                        padding: '8px 10px',
                        borderRadius: '6px',
                        cursor: 'pointer',
                        transition: 'background-color 0.15s ease',
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = '#f1f5f9';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = 'transparent';
                      }}
                    >
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px', overflow: 'hidden' }}>
                        <div style={{
                          width: '26px',
                          height: '26px',
                          borderRadius: '6px',
                          backgroundColor: '#f5f3ff',
                          color: '#7c3aed',
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          flexShrink: 0,
                        }}>
                          <FileCode2 size={14} />
                        </div>
                        <div style={{ overflow: 'hidden' }}>
                          <div style={{ fontSize: '13px', fontWeight: 500, color: 'var(--text-primary)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                            {d.name}
                          </div>
                          {d.workspaceName && (
                            <div style={{ fontSize: '11px', color: 'var(--text-muted)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                              in {d.workspaceName}
                            </div>
                          )}
                        </div>
                      </div>
                      <span style={{
                        fontSize: '10.5px',
                        fontWeight: 600,
                        padding: '2px 6px',
                        borderRadius: '4px',
                        backgroundColor: '#eff6ff',
                        color: '#2563eb',
                        flexShrink: 0,
                      }}>
                        {d.diagramType || 'Diagram'}
                      </span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
