function getPageItems(current, total) {
  const pages = new Set([1, total]);

  for (let page = current - 1; page <= current + 1; page += 1) {
    if (page >= 1 && page <= total) {
      pages.add(page);
    }
  }

  const sorted = [...pages].sort((a, b) => a - b);
  const items = [];

  sorted.forEach((page, index) => {
    if (index > 0 && page - sorted[index - 1] > 1) {
      items.push({ type: 'gap', key: `gap-${page}` });
    }
    items.push({ type: 'page', key: `page-${page}`, page });
  });

  return items;
}

function Pagination({ pageNumber, totalPages, onPageChange }) {
  if (totalPages <= 1) {
    return null;
  }

  const items = getPageItems(pageNumber, totalPages);

  return (
    <nav aria-label="Pagination">
      <ul className="pagination pagination-sm mb-0">
        <li className={`page-item ${pageNumber <= 1 ? 'disabled' : ''}`}>
          <button type="button" className="page-link" onClick={() => onPageChange(pageNumber - 1)} disabled={pageNumber <= 1}>
            Previous
          </button>
        </li>

        {items.map((item) =>
          item.type === 'gap' ? (
            <li key={item.key} className="page-item disabled">
              <span className="page-link">...</span>
            </li>
          ) : (
            <li key={item.key} className={`page-item ${item.page === pageNumber ? 'active' : ''}`}>
              <button
                type="button"
                className="page-link"
                onClick={() => onPageChange(item.page)}
                aria-current={item.page === pageNumber ? 'page' : undefined}
              >
                {item.page}
              </button>
            </li>
          ),
        )}

        <li className={`page-item ${pageNumber >= totalPages ? 'disabled' : ''}`}>
          <button type="button" className="page-link" onClick={() => onPageChange(pageNumber + 1)} disabled={pageNumber >= totalPages}>
            Next
          </button>
        </li>
      </ul>
    </nav>
  );
}

export default Pagination;