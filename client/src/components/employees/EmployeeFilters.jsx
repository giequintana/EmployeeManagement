import { useState } from 'react';
import { useDebouncedCallback } from '@/hooks/useDebouncedCallback';

function EmployeeFilters({ query, departments, hasFilters, onFilterChange, onSearch, onReset }) {
  const [searchText, setSearchText] = useState(query.search);
  const [syncedSearch, setSyncedSearch] = useState(query.search);

  // When the URL changes from outside (Reset, Back button, dashboard link),
  // bring the input in line with it.
  if (query.search !== syncedSearch) {
    setSyncedSearch(query.search);
    if (searchText.trim() !== query.search) {
      setSearchText(query.search);
    }
  }

  const debouncedSearch = useDebouncedCallback((value) => onSearch(value.trim()), 400);

  function handleSearchChange(event) {
    setSearchText(event.target.value);
    debouncedSearch(event.target.value);
  }

  function handleSearchSubmit(event) {
    event.preventDefault();
    onSearch(searchText.trim());
  }

  return (
    <div className="card shadow-sm mb-3">
      <div className="card-body">
        <div className="row g-2 align-items-end">
          <div className="col-12 col-lg-5">
            <form role="search" onSubmit={handleSearchSubmit}>
              <label htmlFor="employee-search" className="form-label small mb-1">Search</label>
              <input
                id="employee-search"
                type="search"
                className="form-control"
                placeholder="Name or employee number"
                value={searchText}
                onChange={handleSearchChange}
                maxLength={100}
              />
            </form>
          </div>

          <div className="col-6 col-lg-3">
            <label htmlFor="department-filter" className="form-label small mb-1">Department</label>
            <select
              id="department-filter"
              className="form-select"
              value={query.departmentId}
              onChange={(event) => onFilterChange({ departmentId: event.target.value })}
            >
              <option value="">All departments</option>
              {departments.map((department) => (
                <option key={department.id} value={String(department.id)}>
                  {department.departmentName}
                </option>
              ))}
            </select>
          </div>

          <div className="col-6 col-lg-2">
            <label htmlFor="status-filter" className="form-label small mb-1">Status</label>
            <select
              id="status-filter"
              className="form-select"
              value={query.isActive}
              onChange={(event) => onFilterChange({ isActive: event.target.value })}
            >
              <option value="">All</option>
              <option value="true">Active</option>
              <option value="false">Inactive</option>
            </select>
          </div>

          <div className="col-12 col-lg-2 d-grid">
            <button type="button" className="btn btn-outline-secondary" onClick={onReset} disabled={!hasFilters}>
              Reset filters
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default EmployeeFilters;