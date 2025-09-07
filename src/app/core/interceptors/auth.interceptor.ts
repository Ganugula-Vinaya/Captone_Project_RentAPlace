import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// Angular 20 uses functional interceptors
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.token;

  if (token) {
    const clone = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(clone);
  }
  return next(req);
};
