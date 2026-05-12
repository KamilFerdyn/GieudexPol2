import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserDto } from '../models/user.dto';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = '/api/Users'; // Endpoint na podstawie UsersController
  
  constructor(private http: HttpClient) {}

  // GET /api/users/{username}
  getUserByUsername(username: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/${username}`);
  }

  // POST /api/users
  createUser(user: UserDto): Observable<UserDto> {
    return this.http.post<UserDto>(this.apiUrl, user);
  }

  // PUT /api/users/{id}
  updateUser(userId: number, user: UserDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${userId}`, user);
  }

  // DELETE /api/users/{id}
  deleteUser(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}`);
  }
}