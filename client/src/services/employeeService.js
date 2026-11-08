import { apiClient } from '@/services/apiClient';
import { cleanParams } from '@/utils/queryParams';

export const employeeService = {
  async getPaged(params, signal) {
    const { data } = await apiClient.get('/employees', {
      params: cleanParams(params),
      signal,
    });
    return data;
  },

  async getById(id, signal) {
    const { data } = await apiClient.get(`/employees/${id}`, { signal });
    return data;
  },

  async create(employee) {
    const { data } = await apiClient.post('/employees', employee);
    return data;
  },

  async update(id, employee) {
    const { data } = await apiClient.put(`/employees/${id}`, employee);
    return data;
  },

  async remove(id) {
    await apiClient.delete(`/employees/${id}`);
  },
};