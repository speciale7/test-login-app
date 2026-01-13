import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CassaIntelligente, CreateCassaInteligenteDto, UpdateCassaInteligenteDto } from '../models/cash-management.model';

@Injectable({
  providedIn: 'root'
})
export class CassaInteligenteService {
  private apiUrl = `${environment.apiUrl}/CassaIntelligente`;

  constructor(private http: HttpClient) { }

  getAll(idStore?: number): Observable<CassaIntelligente[]> {
    let params = new HttpParams();
    if (idStore) {
      params = params.set('idStore', idStore.toString());
    }
    return this.http.get<CassaIntelligente[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<CassaIntelligente> {
    return this.http.get<CassaIntelligente>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateCassaInteligenteDto): Observable<CassaIntelligente> {
    return this.http.post<CassaIntelligente>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateCassaInteligenteDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
