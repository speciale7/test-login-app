import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SovvMonetaria, CreateSovvMonetariaDto, UpdateSovvMonetariaDto } from '../models/cash-management.model';

@Injectable({
  providedIn: 'root'
})
export class SovvMonetariaService {
  private apiUrl = `${environment.apiUrl}/SovvMonetaria`;

  constructor(private http: HttpClient) { }

  getAll(idStore?: number): Observable<SovvMonetaria[]> {
    let params = new HttpParams();
    if (idStore) {
      params = params.set('idStore', idStore.toString());
    }
    return this.http.get<SovvMonetaria[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<SovvMonetaria> {
    return this.http.get<SovvMonetaria>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateSovvMonetariaDto): Observable<SovvMonetaria> {
    return this.http.post<SovvMonetaria>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateSovvMonetariaDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
