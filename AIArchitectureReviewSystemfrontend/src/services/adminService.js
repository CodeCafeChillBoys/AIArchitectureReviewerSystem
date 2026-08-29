import api from './api';

export const adminService = {
  // ==========================================
  // 1. SYSTEM RULES (RAG KNOWLEDGE / ARCH RULES)
  // ==========================================

  /**
   * Lấy toàn bộ danh sách System Rules
   * GET /api/system-rules
   */
  getSystemRules: async () => {
    return api.get('/system-rules');
  },

  /**
   * Lấy chi tiết 1 System Rule theo ID
   * GET /api/system-rules/{id}
   */
  getSystemRuleById: async (id) => {
    return api.get(`/system-rules/${id}`);
  },

  /**
   * Tạo mới 1 System Rule
   * POST /api/system-rules
   * Body: { diagramType: string, ruleName: string, regexOrCondition: string, isActive: boolean }
   */
  createSystemRule: async (ruleData) => {
    return api.post('/system-rules', ruleData);
  },

  /**
   * Cập nhật System Rule
   * PUT /api/system-rules/{id}
   * Body: { diagramType: string, ruleName: string, regexOrCondition: string, isActive: boolean }
   */
  updateSystemRule: async (id, ruleData) => {
    return api.put(`/system-rules/${id}`, ruleData);
  },

  /**
   * Xóa 1 System Rule
   * DELETE /api/system-rules/{id}
   */
  deleteSystemRule: async (id) => {
    return api.delete(`/system-rules/${id}`);
  },

  /**
   * Upload file Markdown / Text làm System Rule
   * POST /api/system-rules/upload
   */
  uploadSystemRuleFile: async (formData) => {
    return api.post('/system-rules/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },

  /**
   * Tự động Seed toàn bộ file từ thư mục RAG_Documents vào DB
   * POST /api/system-rules/seed-from-directory?clearExisting=false
   */
  seedSystemRules: async (clearExisting = false) => {
    return api.post(`/system-rules/seed-from-directory?clearExisting=${clearExisting}`);
  },

  /**
   * Xóa toàn bộ System Rules và embeddings
   * DELETE /api/system-rules/clear-all
   */
  clearAllSystemRules: async () => {
    return api.delete('/system-rules/clear-all');
  },


  // ==========================================
  // 2. PROMPT TEMPLATES MANAGEMENT
  // ==========================================

  /**
   * Lấy toàn bộ danh sách Prompt Templates
   * GET /api/prompts
   */
  getPrompts: async () => {
    return api.get('/prompts');
  },

  /**
   * Lấy chi tiết Prompt Template theo ID
   * GET /api/prompts/{id}
   */
  getPromptById: async (id) => {
    return api.get(`/prompts/${id}`);
  },

  /**
   * Tạo mới 1 Prompt Template
   * POST /api/prompts
   * Body: { name: string, content: string, diagramType: string }
   */
  createPrompt: async (promptData) => {
    return api.post('/prompts', promptData);
  },

  /**
   * Cập nhật Prompt Template
   * PUT /api/prompts/{id}
   * Body: { name?: string, content?: string, diagramType?: string }
   */
  updatePrompt: async (id, promptData) => {
    return api.put(`/prompts/${id}`, promptData);
  },

  /**
   * Xóa Prompt Template
   * DELETE /api/prompts/{id}
   */
  deletePrompt: async (id) => {
    return api.delete(`/prompts/${id}`);
  },


  // ==========================================
  // 3. USER MANAGEMENT
  // ==========================================

  /**
   * Lấy toàn bộ danh sách Users
   * GET /api/users hoặc GET /api/auth/users
   */
  getUsers: async () => {
    try {
      return await api.get('/users');
    } catch (err) {
      if (err.response?.status === 404) {
        return await api.get('/auth/users');
      }
      throw err;
    }
  },

  /**
   * Lấy thông tin user theo ID
   * GET /api/users/{id}
   */
  getUserById: async (id) => {
    try {
      return await api.get(`/users/${id}`);
    } catch (err) {
      if (err.response?.status === 404) {
        return await api.get(`/auth/users/${id}`);
      }
      throw err;
    }
  },

  /**
   * Cập nhật vai trò / quyền hạn của User
   * PUT /api/users/{id}/role
   * Body: { role: 'Admin' | 'User' }
   */
  updateUserRole: async (id, role) => {
    try {
      return await api.put(`/users/${id}/role`, { role });
    } catch (err) {
      if (err.response?.status === 404) {
        return await api.put(`/auth/users/${id}/role`, { role });
      }
      throw err;
    }
  },

  /**
   * Xóa tài khoản User
   * DELETE /api/users/{id}
   */
  deleteUser: async (id) => {
    try {
      return await api.delete(`/users/${id}`);
    } catch (err) {
      if (err.response?.status === 404) {
        return await api.delete(`/auth/users/${id}`);
      }
      throw err;
    }
  },
};
