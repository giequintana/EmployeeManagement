import { useCallback, useEffect, useLayoutEffect, useRef, useState } from 'react';

/**
 * Runs queryFn(signal) whenever deps change, cancels outdated requests,
 * and exposes { data, error, isLoading, reload }.
 * deps must be plain, serializable values (ids, query objects).
 */
export function useApiQuery(queryFn, deps = []) {
  const queryRef = useRef(queryFn);
  const [reloadCount, setReloadCount] = useState(0);
  const [result, setResult] = useState({ key: null, data: null, error: null });

  const requestKey = JSON.stringify([deps, reloadCount]);

  useLayoutEffect(() => {
    queryRef.current = queryFn;
  });

  useEffect(() => {
    const controller = new AbortController();

    queryRef.current(controller.signal)
      .then((data) => setResult({ key: requestKey, data, error: null }))
      .catch((error) => {
        if (!error.isCanceled) {
          setResult({ key: requestKey, data: null, error });
        }
      });

    return () => controller.abort();
  }, [requestKey]);

  const reload = useCallback(() => setReloadCount((count) => count + 1), []);

  const isCurrent = result.key === requestKey;

  return {
    data: result.data,
    error: isCurrent ? result.error : null,
    isLoading: !isCurrent,
    reload,
  };
}