import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  public readonly baseUrl = 'https://localhost:7048/api/';

  constructor(private readonly client: HttpClient) {
  }

  public get<T>(resource: string, params?: any) {
    return this.client.get<T>(this.baseUrl + resource, { params });
  }
}
