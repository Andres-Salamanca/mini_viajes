import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Trip } from '../models/trip.model';
import { CreateTripRequest } from '../models/trip-request.model';
@Injectable({
  providedIn: 'root'
})
export class TripsService {

   private baseUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) {}
  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({ 'Authorization': `Bearer ${token}` })
    };
  }

  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(`${this.baseUrl}/getDestinations`, this.getAuthHeaders());
  }

  createTrip(trip: CreateTripRequest): Observable<Trip> {
    return this.http.post<Trip>(`${this.baseUrl}/createDestination`, trip, this.getAuthHeaders());
  }

  updateTrip(id: string, trip: CreateTripRequest): Observable<Trip> {
    return this.http.put<Trip>(`${this.baseUrl}/updateDestination/${id}`, trip, this.getAuthHeaders());
  }

  deleteTrip(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/deleteDestination/${id}`, this.getAuthHeaders());
  }
}
