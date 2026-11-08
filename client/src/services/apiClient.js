import axios from 'axios';
import { config } from '@/utils/config';
import { toApiError } from '@/utils/apiError';
import { tokenStorage } from '@/services/tokenStorage';

let unauthorizedHandler = null;

export function setUnauthorizedHandler(handler) {
  unauthorizedHandler = handler;
}

export const apiClient = axios.create({
  baseURL: `${config.apiBaseUrl}/api`,
  timeout: 15_000,
  headers: { Accept: 'application/json' },
});

apiClient.interceptors.request.use((request) => {
  const token = tokenStorage.getAccessToken();

  if (token) {
    request.headers.set('Authorization', `Bearer ${token}`);
  }

  return request;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const apiError = toApiError(error);

    if (apiError.status === 401 && !error.config?.skipUnauthorizedHandler) {
      unauthorizedHandler?.(apiError);
    }

    return Promise.reject(apiError);
  },
);