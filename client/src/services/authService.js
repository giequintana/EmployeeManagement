import { apiClient } from '@/services/apiClient';

export const authService = {
  async login(username, password) {
    const { data } = await apiClient.post(
      '/auth/login',
      { username, password },
      { skipUnauthorizedHandler: true },
    );
    return data;
  },

  async getCurrentUser(signal) {
    const { data } = await apiClient.get('/auth/me', { signal });
    return data;
  },
};