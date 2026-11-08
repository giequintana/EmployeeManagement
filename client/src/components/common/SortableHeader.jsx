function SortableHeader({ label, field, sortBy, sortDescending, onSort, className = '' }) {
  const isActive = sortBy === field;
  const ariaSort = isActive ? (sortDescending ? 'descending' : 'ascending') : 'none';
  const indicator = isActive ? (sortDescending ? '▼' : '▲') : '↕';

  return (
    <th scope="col" className={className} aria-sort={ariaSort}>
      <button
        type="button"
        className="btn btn-link p-0 text-reset text-decoration-none fw-semibold text-nowrap"
        onClick={() => onSort(field)}
      >
        {label}
        <span className={`ms-1 small ${isActive ? '' : 'text-body-tertiary'}`} aria-hidden="true">
          {indicator}
        </span>
      </button>
    </th>
  );
}

export default SortableHeader;