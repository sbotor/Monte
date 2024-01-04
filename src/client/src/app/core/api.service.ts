import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment.development';
import { cleanParams } from './apiUtils';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  public readonly baseUrl = environment.apiUrl.toLowerCase();

  constructor(private readonly client: HttpClient) {
  }

  public get<T>(resource: string, params?: any) {
    params = cleanParams(params);

    return this.client.get<T>(this.baseUrl + resource, { params });
  }
}
