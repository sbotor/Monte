import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserRole } from '@auth/roles';
import { environment } from 'environments/environment';

export interface UserDetails {
  id: string;
  name: string;
  role: UserRole;
}

export interface CreateUserRequest {
  username: string;
  password: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

export interface ChangeUsernameRequest {
  newUsername: string;
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

  public getUser(id: string) {
    return this.http.get<UserDetails>(this._baseUrl + id);
  }

  public createUser(request: CreateUserRequest) {
    return this.http.post<UserDetails>(this._baseUrl, request);
  }

  public deleteUser(id: string) {
    return this.http.delete(this._baseUrl + id);
  }

  public changePassword(id: string, request: ChangePasswordRequest) {
    return this.http.post(this._baseUrl + id + '/password', request);
  }

  public changeUsername(id: string, request: ChangeUsernameRequest) {
    return this.http.post(this._baseUrl + id + '/username', request);
  }
}
