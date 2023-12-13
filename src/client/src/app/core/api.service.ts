import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  public readonly baseUrl = 'https://localhost:7048/api/';

  constructor(private readonly client: HttpClient) {
  }

  public get<T>(resource: string, params?: any) {
    if (params) {
      let httpParams = new HttpParams();
      Object.keys(params).forEach(key => {
        const value = params[key];
        if (value !== undefined && value !== null) {
          httpParams = httpParams.append(key, value);
        }
      });

      params = httpParams;
    }

    return this.client.get<T>(this.baseUrl + resource, { params });
  }
}
