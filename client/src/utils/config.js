const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

if (!apiBaseUrl) {
  throw new Error('VITE_API_BASE_URL is not set. Add it to client/.env.development.');
}

export const config = Object.freeze({
  apiBaseUrl,
});

const allowedProtocols = ['https:', 'http:'];