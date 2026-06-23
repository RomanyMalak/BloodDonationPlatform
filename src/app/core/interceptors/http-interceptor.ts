import { HttpInterceptorFn } from '@angular/common/http';

export const httpInterceptor: HttpInterceptorFn = (req, next) => {
  // Don't set Content-Type for FormData - let browser handle it
  if (!(req.body instanceof FormData)) {
    req = req.clone({
      setHeaders: {
        'Content-Type': 'application/json'
      }
    });
  }
  return next(req);
};
