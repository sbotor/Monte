import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment.development';

export interface UserDetails {
  id: string;
  name: string;
}

export interface CreateUserRequest {
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  public readonly _baseUrl = `${environment.authRootUrl}api/users`;

  constructor(private readonly http: HttpClient) { }

  public getUsers() {
    return this.http.get<UserDetails[]>(this._baseUrl);
  }

  public createUser(request: CreateUserRequest) {
    return this.http.post<UserDetails>(this._baseUrl, request);
  }
}
