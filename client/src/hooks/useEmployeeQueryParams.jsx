import { useCallback, useMemo } from 'react';
import { useSearchParams } from 'react-router';

export const PAGE_SIZE_OPTIONS = [10, 25, 50];

const DEFAULTS = {
  search: '',
  departmentId: '',
  isActive: '',
  sortBy: 'lastName',
  sortDescending: false,
  pageNumber: 1,
  pageSize: 10,
};

function toPositiveInt(value, fallback) {
  const number = Number.parseInt(value, 10);
  return Number.isInteger(number) && number > 0 ? number : fallback;
}

function isDefaultValue(key, value) {
  return value === '' || value === null || value === undefined || String(value) === String(DEFAULTS[key]);
}

export function useEmployeeQueryParams() {
  const [searchParams, setSearchParams] = useSearchParams();

  const query = useMemo(() => {
    const isActive = searchParams.get('isActive');
    const pageSize = toPositiveInt(searchParams.get('pageSize'), DEFAULTS.pageSize);

    return {
      search: searchParams.get('search') ?? DEFAULTS.search,
      departmentId: searchParams.get('departmentId') ?? DEFAULTS.departmentId,
      isActive: isActive === 'true' || isActive === 'false' ? isActive : DEFAULTS.isActive,
      sortBy: searchParams.get('sortBy') ?? DEFAULTS.sortBy,
      sortDescending: searchParams.get('sortDescending') === 'true',
      pageNumber: toPositiveInt(searchParams.get('pageNumber'), DEFAULTS.pageNumber),
      pageSize: PAGE_SIZE_OPTIONS.includes(pageSize) ? pageSize : DEFAULTS.pageSize,
    };
  }, [searchParams]);

  const updateQuery = useCallback(
    (changes, { resetPage = true, replace = false } = {}) => {
      setSearchParams(
        (current) => {
          const next = new URLSearchParams(current);
          const merged = resetPage && !('pageNumber' in changes)
            ? { ...changes, pageNumber: DEFAULTS.pageNumber }
            : changes;

          Object.entries(merged).forEach(([key, value]) => {
            if (isDefaultValue(key, value)) {
              next.delete(key);
            } else {
              next.set(key, String(value));
            }
          });

          return next;
        },
        { replace },
      );
    },
    [setSearchParams],
  );

  const resetQuery = useCallback(() => setSearchParams({}), [setSearchParams]);

  return { query, updateQuery, resetQuery };
}