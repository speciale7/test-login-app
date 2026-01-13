import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Busta, CreateBustaDto, UpdateBustaDto } from '../models/busta.model';

@Injectable({
  providedIn: 'root'
})
export class BusteService {
  private apiUrl = `${environment.apiUrl}/buste`;

  constructor(private http: HttpClient) { }

  getBuste(startDate?: Date, endDate?: Date): Observable<Busta[]> {
    let params = new HttpParams();
    
    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }

    return this.http.get<Busta[]>(this.apiUrl, { params });
  }

  getBusta(id: number): Observable<Busta> {
    return this.http.get<Busta>(`${this.apiUrl}/${id}`);
  }

  createBusta(busta: CreateBustaDto): Observable<Busta> {
    return this.http.post<Busta>(this.apiUrl, busta);
  }

  updateBusta(id: number, busta: UpdateBustaDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, busta);
  }

  deleteBusta(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  duplicateBusta(id: number): Observable<Busta> {
    return this.http.post<Busta>(`${this.apiUrl}/${id}/duplicate`, {});
  }
}
