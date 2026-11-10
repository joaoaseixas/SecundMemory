import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { CapacitySummary } from '../models/capacity-summary.model';

@Injectable({ providedIn: 'root' })
export class SecondBrainApiService {
  private readonly http = inject(HttpClient);

  getCapacitySummary(): Observable<CapacitySummary> {
    return this.http.get<CapacitySummary>(`${API_BASE_URL}/capacity`);
  }
}
