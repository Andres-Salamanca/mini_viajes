import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest, RegisterRequest } from '../models/auth.models';
@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private baseUrl = 'http://localhost:5062';

  constructor(private http: HttpClient) {}

  login(payload: LoginRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/logIn`, payload);
  }

  register(payload: RegisterRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/createUser`, payload);
  }

}
