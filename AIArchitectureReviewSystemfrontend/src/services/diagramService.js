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
   * POST /api/diagrams/mermaid
   * Body: { workspaceId: string, name: string, content: string }
   */
  createMermaidDiagram: async (data) => {
    return api.post('/diagrams/mermaid', data);
  },

  /**
   * Upload file Diagram (.mmd / .png / .jpg / .json)
   * POST /api/diagrams/upload (multipart/form-data)
   * Form: file, workspaceId, diagramType
   */
  uploadDiagram: async (formData) => {
    return api.post('/diagrams/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },
};
