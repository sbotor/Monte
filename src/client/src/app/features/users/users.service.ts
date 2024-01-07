import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserRole } from '@auth/roles';
import { environment } from 'environments/environment.development';

export interface UserDetails {
  id: string;
  name: string;
  role: UserRole;
}

export interface CreateUserRequest {
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  public readonly _baseUrl = `${environment.authRootUrl}api/users/`;

  constructor(private readonly http: HttpClient) { }

  public getUsers() {
    return this.http.get<UserDetails[]>(this._baseUrl);
  }

  public createUser(request: CreateUserRequest) {
    return this.http.post<UserDetails>(this._baseUrl, request);
  }

  public deleteUser(id: string) {
    return this.http.delete(this._baseUrl + id);
  }
}
