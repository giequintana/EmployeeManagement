import axios from 'axios';

const STATUS_MESSAGES = {
  400: 'Please check the highlighted fields.',
  401: 'Your session has expired. Please sign in again.',
  403: 'You do not have permission to perform this action.',
  404: 'The requested item was not found.',
  409: 'This change conflicts with existing data.',
  500: 'Something went wrong on the server. Please try again.',
  429: 'Too many attempts. Please wait a moment and try again.',
};

export class ApiError extends Error {
  constructor({
    message,
    status = 0,
    errorCode = null,
    fieldErrors = {},
    traceId = null,
    isNetworkError = false,
    isCanceled = false,
  }) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.errorCode = errorCode;
    this.fieldErrors = fieldErrors;
    this.traceId = traceId;
    this.isNetworkError = isNetworkError;
    this.isCanceled = isCanceled;
  }

  get isValidationError() {
    return this.status === 400 && Object.keys(this.fieldErrors).length > 0;
  }
}

function normalizeFieldName(key) {
  const name = key.startsWith('$.') ? key.slice(2) : key;
  return name.charAt(0).toLowerCase() + name.slice(1);
}

function toFieldErrors(errors) {
  if (!errors || typeof errors !== 'object') {
    return {};
  }

  return Object.fromEntries(
    Object.entries(errors).map(([key, messages]) => [
      normalizeFieldName(key),
      Array.isArray(messages) ? messages : [String(messages)],
    ]),
  );
}

export function toApiError(error) {
  if (error instanceof ApiError) {
    return error;
  }

  if (axios.isCancel(error)) {
    return new ApiError({ message: 'The request was canceled.', isCanceled: true });
  }

  if (!error.response) {
    return new ApiError({
      message: 'Cannot reach the server. Check your connection and try again.',
      isNetworkError: true,
    });
  }

  const { status, data } = error.response;
  const problem = data && typeof data === 'object' ? data : {};

  return new ApiError({
    message:
      problem.detail ||
      STATUS_MESSAGES[status] ||
      problem.title ||
      `Request failed with status ${status}.`,
    status,
    errorCode: problem.errorCode ?? null,
    fieldErrors: toFieldErrors(problem.errors),
    traceId: problem.traceId ?? null,
  });
}