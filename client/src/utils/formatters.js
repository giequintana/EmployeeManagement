export function formatDate(isoDate) {
  if (!isoDate) {
    return '';
  }

  const [year, month, day] = isoDate.split('-').map(Number);

  return new Date(year, month - 1, day).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

export function formatDateTime(value) {
  if (!value) {
    return '';
  }

  // The API stores UTC but may serialize without a "Z"; treat that as UTC.
  const hasTimeZone = /([zZ]|[+-]\d{2}:\d{2})$/.test(value);

  return new Date(hasTimeZone ? value : `${value}Z`).toLocaleString(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  });
}

export function formatMoney(value) {
  return Number(value).toLocaleString(undefined, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}