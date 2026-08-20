import api from './api';

export const workspaceService = {
  /**
   * Lấy danh sách workspaces của một User (có phân trang)
   * GET /api/workspaces/user/{userId}?pageIndex=1&pageSize=10
   */
  getUserWorkspaces: async (userId, pageIndex = 1, pageSize = 10) => {
    return api.get(`/workspaces/user/${userId}`, {
      params: {
        pageIndex,
        pageSize,
      },
    });
  },

  /**
   * Lấy chi tiết 1 Workspace theo ID
   * GET /api/workspaces/{id}
   */
  getWorkspaceById: async (id) => {
    return api.get(`/workspaces/${id}`);
  },

  /**
   * Tạo mới 1 Workspace
   * POST /api/workspaces
   * Body: { name: string, userId: string }
   */
  createWorkspace: async (data) => {
    return api.post('/workspaces', {
      name: data.name,
      userId: data.userId,
    });
  },

  /**
   * Cập nhật thông tin Workspace
   * PUT /api/workspaces/{id}
   * Body: { name: string }
   */
  updateWorkspace: async (id, data) => {
    return api.put(`/workspaces/${id}`, {
      name: data.name,
    });
  },
};
