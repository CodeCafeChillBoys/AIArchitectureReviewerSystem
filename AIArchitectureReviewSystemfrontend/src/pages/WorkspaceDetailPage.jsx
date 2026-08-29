import React, { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Loader2, AlertCircle, RefreshCw, LayoutList, LayoutGrid, Layers } from 'lucide-react';
import { workspaceService } from '../services/workspaceService';
import { diagramService } from '../services/diagramService';

import WorkspaceHeader from '../components/workspace/WorkspaceHeader';
import DiagramCard from '../components/workspace/DiagramCard';
import DiagramListView from '../components/workspace/DiagramListView';
import EmptyDiagramState from '../components/workspace/EmptyDiagramState';
import CreateMermaidModal from '../components/workspace/CreateMermaidModal';
import UploadDiagramModal from '../components/workspace/UploadDiagramModal';

export default function WorkspaceDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  // State chế độ hiển thị: 'table' (bảng/danh sách giống mẫu) hoặc 'grid' (thẻ card)
  const [viewMode, setViewMode] = useState('table');

  // State thông tin Workspace & Diagrams
  const [workspace, setWorkspace] = useState(null);
  const [diagrams, setDiagrams] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // State Modal Tạo Mermaid Diagram
  const [isMermaidModalOpen, setIsMermaidModalOpen] = useState(false);
  const [creatingMermaid, setCreatingMermaid] = useState(false);
  const [mermaidError, setMermaidError] = useState('');

  // State Modal Upload Diagram
  const [isUploadModalOpen, setIsUploadModalOpen] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [uploadError, setUploadError] = useState('');

  // 1. Tải thông tin Workspace và danh sách Diagrams
  const loadWorkspaceData = useCallback(async () => {
    if (!id) return;
    try {
      setLoading(true);
      setError(null);

      // Gọi song song API lấy chi tiết Workspace và danh sách Diagram
      const [wsRes, diagramsRes] = await Promise.allSettled([
        workspaceService.getWorkspaceById(id),
        diagramService.getWorkspaceDiagrams(id, 1, 50),
      ]);

      // Xử lý thông tin Workspace
      if (wsRes.status === 'fulfilled' && wsRes.value?.data) {
        setWorkspace(wsRes.value.data);
      } else if (wsRes.status === 'fulfilled' && wsRes.value?.id) {
        setWorkspace(wsRes.value);
      } else {
        setWorkspace({ id, name: 'Workspace' });
      }

      // Xử lý danh sách Diagrams
      if (diagramsRes.status === 'fulfilled') {
        const dData = diagramsRes.value;
        if (dData?.data?.items) {
          setDiagrams(dData.data.items);
        } else if (Array.isArray(dData?.data)) {
          setDiagrams(dData.data);
        } else if (Array.isArray(dData)) {
          setDiagrams(dData);
        } else {
          setDiagrams([]);
        }
      } else {
        setDiagrams([]);
      }
    } catch (err) {
      console.error('Lỗi khi tải dữ liệu Workspace:', err);
      setError(err.response?.data?.message || err.message || 'Không thể tải dữ liệu Workspace.');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    loadWorkspaceData();
  }, [loadWorkspaceData]);

  // 2. Xử lý Tạo Diagram bằng Mermaid Code
  const handleCreateMermaid = async ({ name, diagramType, description, mermaidCode }) => {
    try {
      setCreatingMermaid(true);
      setMermaidError('');

      const res = await diagramService.createMermaidDiagram({
        workspaceId: id,
        name,
        diagramType,
        description,
        mermaidCode,
      });

      if (res && (res.success || res.data)) {
        setIsMermaidModalOpen(false);
        const createdDiagram = res.data;
        await loadWorkspaceData();
        if (createdDiagram?.id) {
          navigate(`/editor/${createdDiagram.id}`);
        }
      } else {
        setMermaidError(res?.message || 'Không thể tạo sơ đồ Mermaid.');
      }
    } catch (err) {
      console.error('Lỗi khi tạo Mermaid Diagram:', err);
      setMermaidError(err.response?.data?.message || err.message || 'Đã có lỗi xảy ra khi tạo sơ đồ.');
    } finally {
      setCreatingMermaid(false);
    }
  };

  // 3. Xử lý Upload Diagram dạng ảnh
  const handleUploadDiagram = async ({ name, diagramType, description, imageFile }) => {
    try {
      setUploading(true);
      setUploadError('');

      const res = await diagramService.uploadDiagram({
        workspaceId: id,
        name,
        diagramType,
        description,
        imageFile,
      });

      if (res && (res.success || res.data)) {
        setIsUploadModalOpen(false);
        await loadWorkspaceData();
      } else {
        setUploadError(res?.message || 'Không thể tải lên sơ đồ.');
      }
    } catch (err) {
      console.error('Lỗi khi upload Diagram:', err);
      setUploadError(err.response?.data?.message || err.message || 'Đã có lỗi xảy ra khi tải file.');
    } finally {
      setUploading(false);
    }
  };

  // 4. Xử lý Xóa Diagram
  const handleDeleteDiagram = async (diagramId) => {
    try {
      await diagramService.deleteDiagram(diagramId);
      await loadWorkspaceData();
    } catch (err) {
      console.error('Lỗi khi xóa Diagram:', err);
      alert(err.response?.data?.message || err.message || 'Không thể xóa sơ đồ.');
    }
  };

  return (
    <div style={{ padding: '32px 28px', maxWidth: '1400px', margin: '0 auto' }}>
      {/* 1. Header Toolbar của Workspace */}
      <WorkspaceHeader
        workspace={workspace}
        totalDiagrams={diagrams.length}
        onOpenMermaidModal={() => {
          setMermaidError('');
          setIsMermaidModalOpen(true);
        }}
        onOpenUploadModal={() => {
          setUploadError('');
          setIsUploadModalOpen(true);
        }}
      />

      {/* 2. Loading state */}
      {loading && (
        <div style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '80px 0',
          gap: '10px',
          color: 'var(--text-secondary)',
          fontSize: '14px',
        }}>
          <Loader2 size={22} className="animate-spin" color="var(--accent-primary)" />
          <span>Loading architecture diagrams...</span>
        </div>
      )}

      {/* 3. Error state */}
      {!loading && error && (
        <div style={{
          padding: '16px',
          backgroundColor: 'var(--color-danger-bg)',
          border: '1px solid #fecaca',
          borderRadius: 'var(--radius-md)',
          color: 'var(--color-danger)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          marginBottom: '24px',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
            <AlertCircle size={18} />
            <span>{error}</span>
          </div>
          <button
            className="btn btn-secondary btn-sm"
            onClick={loadWorkspaceData}
            style={{ gap: '6px' }}
          >
            <RefreshCw size={14} />
            <span>Retry</span>
          </button>
        </div>
      )}

      {/* 4. Diagrams Content */}
      {!loading && !error && (
        <>
          {diagrams.length === 0 ? (
            <EmptyDiagramState
              onOpenMermaidModal={() => {
                setMermaidError('');
                setIsMermaidModalOpen(true);
              }}
              onOpenUploadModal={() => {
                setUploadError('');
                setIsUploadModalOpen(true);
              }}
            />
          ) : (
            <div>
              {/* Project Diagrams Header Toolbar */}
              <div style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                marginBottom: '16px',
              }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                  <h2 style={{ fontSize: '18px', fontWeight: 700, color: 'var(--text-primary)' }}>
                    Project Diagrams
                  </h2>
                  <span style={{
                    fontSize: '12px',
                    fontWeight: 700,
                    padding: '2px 8px',
                    borderRadius: '12px',
                    backgroundColor: 'var(--accent-blue-light)',
                    color: 'var(--accent-primary)',
                  }}>
                    {diagrams.length}
                  </span>
                </div>

                {/* View Mode Toggle Buttons */}
                <div style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: '4px',
                  backgroundColor: '#ffffff',
                  padding: '3px',
                  borderRadius: 'var(--radius-sm)',
                  border: '1px solid var(--border-color)',
                }}>
                  <button
                    onClick={() => setViewMode('table')}
                    title="Table View"
                    style={{
                      border: 'none',
                      backgroundColor: viewMode === 'table' ? 'var(--accent-blue-light)' : 'transparent',
                      color: viewMode === 'table' ? 'var(--accent-primary)' : 'var(--text-muted)',
                      padding: '6px 8px',
                      borderRadius: '4px',
                      cursor: 'pointer',
                      display: 'flex',
                      alignItems: 'center',
                    }}
                  >
                    <LayoutList size={16} />
                  </button>

                  <button
                    onClick={() => setViewMode('grid')}
                    title="Grid View"
                    style={{
                      border: 'none',
                      backgroundColor: viewMode === 'grid' ? 'var(--accent-blue-light)' : 'transparent',
                      color: viewMode === 'grid' ? 'var(--accent-primary)' : 'var(--text-muted)',
                      padding: '6px 8px',
                      borderRadius: '4px',
                      cursor: 'pointer',
                      display: 'flex',
                      alignItems: 'center',
                    }}
                  >
                    <LayoutGrid size={16} />
                  </button>
                </div>
              </div>

              {/* Render Table or Grid */}
              {viewMode === 'table' ? (
                <DiagramListView diagrams={diagrams} onDelete={handleDeleteDiagram} />
              ) : (
                <div style={{
                  display: 'grid',
                  gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))',
                  gap: '20px',
                }}>
                  {diagrams.map((diagram) => (
                    <DiagramCard key={diagram.id} diagram={diagram} onDelete={handleDeleteDiagram} />
                  ))}
                </div>
              )}
            </div>
          )}
        </>
      )}

      {/* 5. Modal Tạo Mermaid Diagram */}
      <CreateMermaidModal
        isOpen={isMermaidModalOpen}
        onClose={() => setIsMermaidModalOpen(false)}
        onSubmit={handleCreateMermaid}
        loading={creatingMermaid}
        error={mermaidError}
      />

      {/* 6. Modal Upload Diagram Hình ảnh */}
      <UploadDiagramModal
        isOpen={isUploadModalOpen}
        onClose={() => setIsUploadModalOpen(false)}
        onSubmit={handleUploadDiagram}
        loading={uploading}
        error={uploadError}
      />
    </div>
  );
}
