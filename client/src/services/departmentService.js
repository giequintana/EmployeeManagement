import { apiClient } from '@/services/apiClient';

export const departmentService = {
  async getAll(signal) {
    const { data } = await apiClient.get('/departments', { signal });
    return data;
  },

  async getById(id, signal) {
    const { data } = await apiClient.get(`/departments/${id}`, { signal });
    return data;
  },

  async create(department) {
    const { data } = await apiClient.post('/departments', department);
    return data;
  },

  async update(id, department) {
    const { data } = await apiClient.put(`/departments/${id}`, department);
    return data;
  },

  async remove(id) {
    await apiClient.delete(`/departments/${id}`);
  },
};