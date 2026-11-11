/** Matches the shape ExceptionHandlingMiddleware (backend) writes for non-2xx responses. */
export interface ApiError {
  title: string;
  errors?: Record<string, string[]>;
}
