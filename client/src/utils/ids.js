export function parsePositiveId(value) {
  const number = Number(value);
  return Number.isInteger(number) && number > 0 ? number : null;
}