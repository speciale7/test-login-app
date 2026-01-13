import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { FondoCassa, CreateFondoCassaDto, UpdateFondoCassaDto } from '../models/cash-management.model';

@Injectable({
  providedIn: 'root'
})
export class FondoCassaService {
  private apiUrl = `${environment.apiUrl}/FondoCassa`;

  constructor(private http: HttpClient) { }

  getAll(idStore?: number, cashCode?: string): Observable<FondoCassa[]> {
    let params = new HttpParams();
    if (idStore) {
      params = params.set('idStore', idStore.toString());
    }
    if (cashCode) {
      params = params.set('cashCode', cashCode);
    }
    return this.http.get<FondoCassa[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<FondoCassa> {
    return this.http.get<FondoCassa>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateFondoCassaDto): Observable<FondoCassa> {
    return this.http.post<FondoCassa>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateFondoCassaDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
