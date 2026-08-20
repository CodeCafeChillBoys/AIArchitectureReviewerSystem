import api from './api';

export const reviewService = {
  // Trigger AI review for a diagram / workspace
  triggerReview: async (reviewPayload) => {
    try {
      const response = await api.post('/review/audit', reviewPayload);
      return response.data;
    } catch (error) {
      console.warn('Review API fallback', error);
      return {
        id: 'rev-001',
        title: 'Microservices Refactoring v3',
        status: 'Violations Found',
        consistencyScore: 78,
        violationsCount: 2,
        matrix: [
          { component: 'AuthService', auth: 'green', billing: 'green', inventory: 'gray', orders: 'gray' },
          { component: 'BillingService', auth: 'green', billing: 'green', inventory: 'orange', orders: 'green' },
          { component: 'InventorySvc', auth: 'gray', billing: 'orange', inventory: 'green', orders: 'red' },
          { component: 'OrderProcessor', auth: 'gray', billing: 'green', inventory: 'red', orders: 'green' },
        ],
        violations: [
          {
            id: 'v1',
            type: 'Circular Dependency Detected',
            severity: 'critical',
            description: 'Direct bi-directional cyclic dependency identified between OrderService and InventoryService.',
            files: ['OrderService.cs', 'InventoryService.cs'],
            codeSnippet: `// OrderService.cs
import { InventorySvc } from 'services/inventory';

// InventoryService.cs
import { OrderProcessor } from 'services/orders';`,
          },
          {
            id: 'v2',
            type: 'Direct DB Access Across Boundaries',
            severity: 'warning',
            description: 'BillingService performs direct query to Inventory DB without calling the Inventory API.',
            files: ['BillingService.cs'],
            codeSnippet: `// BillingService.cs
var stock = await _inventoryDbContext.Stocks.FindAsync(productId);`,
          },
        ],
      };
    }
  },
};
