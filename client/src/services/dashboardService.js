import { apiClient } from '@/services/apiClient';

export const dashboardService = {
  async getSummary(signal) {
    const { data } = await apiClient.get('/dashboard/summary', { signal });
    return data;
  },
};