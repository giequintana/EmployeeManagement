import { useCallback, useEffect, useMemo, useState } from 'react';
import { AuthContext } from '@/contexts/AuthContext';
import { setUnauthorizedHandler } from '@/services/apiClient';
import { authService } from '@/services/authService';
import { tokenStorage } from '@/services/tokenStorage';

export function AuthProvider({ children }) {
  const [session, setSession] = useState(() => tokenStorage.load());
  const [sessionExpired, setSessionExpired] = useState(false);

  const login = useCallback(async (username, password) => {
    const response = await authService.login(username, password);

    const newSession = {
      accessToken: response.accessToken,
      expiresAtUtc: response.expiresAtUtc,
      user: response.user,
    };

    tokenStorage.save(newSession);
    setSessionExpired(false);
    setSession(newSession);

    return newSession.user;
  }, []);

  const logout = useCallback(() => {
    tokenStorage.clear();
    setSessionExpired(false);
    setSession(null);
  }, []);

  const expireSession = useCallback(() => {
    tokenStorage.clear();
    setSessionExpired(true);
    setSession(null);
  }, []);

  // Any 401 from the API (other than login) ends the session.
  useEffect(() => {
    setUnauthorizedHandler(expireSession);
    return () => setUnauthorizedHandler(null);
  }, [expireSession]);

  // Log out exactly when the token expires, even if the user is idle.
  useEffect(() => {
    if (!session) {
      return undefined;
    }

    const msUntilExpiry = Date.parse(session.expiresAtUtc) - Date.now();
    const timer = setTimeout(expireSession, Math.max(msUntilExpiry, 0));

    return () => clearTimeout(timer);
  }, [session, expireSession]);

  const value = useMemo(() => {
    const user = session?.user ?? null;

    return {
      user,
      isAuthenticated: user !== null,
      isAdmin: user?.role === 'Admin',
      sessionExpired,
      login,
      logout,
    };
  }, [session, sessionExpired, login, logout]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}