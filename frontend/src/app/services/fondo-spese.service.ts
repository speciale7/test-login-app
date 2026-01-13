import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { FondoSpese, CreateFondoSpeseDto, UpdateFondoSpeseDto } from '../models/cash-management.model';

@Injectable({
  providedIn: 'root'
})
export class FondoSpeseService {
  private apiUrl = `${environment.apiUrl}/FondoSpese`;

  constructor(private http: HttpClient) { }

  getAll(idStore?: number): Observable<FondoSpese[]> {
    let params = new HttpParams();
    if (idStore) {
      params = params.set('idStore', idStore.toString());
    }
    return this.http.get<FondoSpese[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<FondoSpese> {
    return this.http.get<FondoSpese>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateFondoSpeseDto): Observable<FondoSpese> {
    return this.http.post<FondoSpese>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateFondoSpeseDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
