import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { of, tap } from 'rxjs';
import { CacheService } from '../services/cache.service';
import { environment } from '@environments/environment';

export const cacheInterceptor: HttpInterceptorFn = (req, next) => {
  const cacheService = inject(CacheService);

  // Only cache GET requests
  if (req.method !== 'GET') {
    return next(req);
  }

  // Don't cache health checks
  if (req.url.includes('/health')) {
    return next(req);
  }

  const cacheKey = req.urlWithParams;
  const cachedResponse = cacheService.get<HttpResponse<any>>(cacheKey);

  if (cachedResponse) {
    return of(cachedResponse);
  }

  return next(req).pipe(
    tap((event) => {
      if (event instanceof HttpResponse) {
        cacheService.set(cacheKey, event, environment.cacheTimeout);
      }
    })
  );
};
