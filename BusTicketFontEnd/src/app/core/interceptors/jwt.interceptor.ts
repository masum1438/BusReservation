import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router, CanActivateFn } from "@angular/router";
import { catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "../services/auth.service";


// ── JWT Interceptor ────────────────────────────────────────────────────────────
export const jwtInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  const token  = auth.getToken();

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401) {
        return auth.refreshToken().pipe(
          switchMap(res => {
            if (res.isSuccess) {
              const retried = req.clone({ setHeaders: { Authorization: `Bearer ${res.token}` } });
              return next(retried);
            }
            auth.logout();
            return throwError(() => err);
          }),
          catchError(() => {
            auth.logout();
            return throwError(() => err);
          })
        );
      }
      return throwError(() => err);
    })
  );
};

// ── Auth Guard ─────────────────────────────────────────────────────────────────
export const authGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  if (auth.isLoggedIn()) return true;
  router.navigate(['/login']);
  return false;
};

// ── Admin Guard ────────────────────────────────────────────────────────────────
export const adminGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  if (auth.isLoggedIn() && auth.isAdmin()) return true;
  router.navigate(['/']);
  return false;
};
