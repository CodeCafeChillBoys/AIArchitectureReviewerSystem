import api from './api';

export const diagramService = {
  // Get diagrams for a workspace
  getDiagrams: async (workspaceId) => {
    try {
      const response = await api.get(`/diagrams?workspaceId=${workspaceId}`);
      return response.data;
    } catch (error) {
      console.warn('Backend API not reachable, using fallback mock data', error);
      return [
        {
          id: '1',
          name: 'System Overview',
          fileName: 'system_overview.mmd',
          format: 'mermaid',
          version: 'v1.4.2',
          updatedAt: 'Today, 10:43 AM',
          status: 'Consistent',
        },
        {
          id: '2',
          name: 'Database Schema',
          fileName: 'db_schema.png',
          format: 'image',
          version: 'v1.0.1',
          updatedAt: 'Yesterday, 4:15 PM',
          status: 'Consistent',
        },
        {
          id: '3',
          name: 'API Flow',
          fileName: 'api_flow_v2.mmd',
          format: 'mermaid',
          version: 'v2.1.0',
          updatedAt: 'Oct 12, 2026',
          status: 'Review Needed',
        },
      ];
    }
  },

  // Get specific diagram content
  getDiagramById: async (id) => {
    try {
      const response = await api.get(`/diagrams/${id}`);
      return response.data;
    } catch (error) {
      return {
        id,
        name: 'Microservices Architecture v3',
        content: `graph TD
  A[Client] --> B[API Gateway]
  B --> C[Auth Service]
  B --> D[Order Service]
  B --> E[Inventory Service]
  D --> E
  E --> D
  D --> F[(Order DB)]
  E --> G[(Inventory DB)]
  style D fill:#1e293b,stroke:#ef4444,stroke-width:2px
  style E fill:#1e293b,stroke:#ef4444,stroke-width:2px`,
      };
    }
  },

  // Save / Update diagram
  saveDiagram: async (diagramData) => {
    try {
      const response = await api.post('/diagrams', diagramData);
      return response.data;
    } catch (error) {
      return { success: true, message: 'Saved successfully (Local Mock)' };
    }
  },
};
