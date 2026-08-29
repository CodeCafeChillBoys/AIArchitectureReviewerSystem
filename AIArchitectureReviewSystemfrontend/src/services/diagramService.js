import api from './api';

export const diagramService = {
  /**
   * Lấy danh sách Diagrams thuộc một Workspace
   * GET /api/diagrams/workspace/{workspaceId}?pageIndex=1&pageSize=10
   */
  getWorkspaceDiagrams: async (workspaceId, pageIndex = 1, pageSize = 10) => {
    return api.get(`/diagrams/workspace/${workspaceId}`, {
      params: {
        pageIndex,
        pageSize,
      },
    });
  },

  /**
   * Lấy chi tiết một Diagram theo ID
   * GET /api/diagrams/{id}
   */
  getDiagramById: async (id) => {
    return api.get(`/diagrams/${id}`);
  },

  /**
   * Tạo mới Diagram bằng mã Mermaid
   * POST /api/diagrams/mermaid (application/json)
   * @param {Object} data
   * @param {string} data.workspaceId - ID của Workspace (Guid)
   * @param {string} data.name - Tên sơ đồ
   * @param {string} data.diagramType - Loại sơ đồ (vd: "Flowchart", "Sequence", "Class", "ERD")
   * @param {string} [data.description] - Mô tả sơ đồ
   * @param {string} data.mermaidCode - Chuỗi mã nguồn Mermaid (vd: "graph TD\n A --> B")
   */
  createMermaidDiagram: async ({
    workspaceId,
    name,
    diagramType,
    description = '',
    mermaidCode,
  }) => {
    return api.post('/diagrams/mermaid', {
      workspaceId,
      name,
      diagramType,
      description,
      mermaidCode,
    });
  },

  /**
   * Upload file Diagram hình ảnh (.png / .jpg / .jpeg)
   * POST /api/diagrams/upload (multipart/form-data)
   * @param {Object|FormData} data
   * @param {string} data.workspaceId - ID của Workspace (Guid)
   * @param {string} data.name - Tên sơ đồ
   * @param {string} data.diagramType - Loại sơ đồ (vd: "Flowchart", "Sequence", "Class", "ERD")
   * @param {string} [data.description] - Mô tả sơ đồ
   * @param {File} data.imageFile - File ảnh upload
   */
  uploadDiagram: async (data) => {
    let formData;
    if (data instanceof FormData) {
      formData = data;
    } else {
      formData = new FormData();
      formData.append('WorkspaceId', data.workspaceId);
      formData.append('Name', data.name);
      formData.append('DiagramType', data.diagramType);
      if (data.description) formData.append('Description', data.description);
      if (data.imageFile) formData.append('ImageFile', data.imageFile);
    }

    return api.post('/diagrams/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },

  /**
   * Xóa một Diagram theo ID
   * DELETE /api/diagrams/{id}
   */
  deleteDiagram: async (id) => {
    return api.delete(`/diagrams/${id}`);
  },
};
