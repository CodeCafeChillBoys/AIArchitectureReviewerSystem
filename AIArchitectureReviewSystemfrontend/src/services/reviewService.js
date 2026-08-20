import api from './api';

export const reviewService = {
  /**
   * 2. Lấy AI Report của một phiên bản sơ đồ theo VersionId
   * GET /api/reviews/versions/{versionId}/report
   */
  getReportByVersionId: async (versionId) => {
    return api.get(`/reviews/versions/${versionId}/report`);
  },

  /**
   * 3. Gửi tin nhắn chat tới AI Architecture Assistant
   * POST /api/reviews/{sessionId}/chat
   * Body: { message: string }
   */
  sendChatMessage: async (sessionId, message) => {
    return api.post(`/reviews/${sessionId}/chat`, { message });
  },

  /**
   * 4. Lấy lịch sử chat theo SessionId
   * GET /api/reviews/{sessionId}/chat
   */
  getChatHistory: async (sessionId) => {
    return api.get(`/reviews/${sessionId}/chat`);
  },

  /**
   * 5. Lấy danh sách System Rules (Quy tắc kiến trúc RAG)
   * GET /api/system-rules
   */
  getSystemRules: async () => {
    return api.get('/system-rules');
  },
};
