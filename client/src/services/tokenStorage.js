const STORAGE_KEY = 'ems.session';
const EXPIRY_BUFFER_MS = 30_000;

function isExpired(expiresAtUtc) {
  const expiresAt = Date.parse(expiresAtUtc);
  return Number.isNaN(expiresAt) || expiresAt - EXPIRY_BUFFER_MS <= Date.now();
}

export const tokenStorage = {
  save(session) {
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(session));
  },

  load() {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return null;
    }

    try {
      const session = JSON.parse(raw);

      if (!session?.accessToken || isExpired(session.expiresAtUtc)) {
        this.clear();
        return null;
      }

      return session;
    } catch {
      this.clear();
      return null;
    }
  },

  getAccessToken() {
    return this.load()?.accessToken ?? null;
  },

  clear() {
    sessionStorage.removeItem(STORAGE_KEY);
  },
};