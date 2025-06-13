import { Component } from '@angular/core';
import { Trip } from '../../models/trip.model';
import { TripsService  } from '../../services/trips.service';
@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent {
  trips: Trip[] = [];
  constructor(private tripsService: TripsService) {}
  ngOnInit(): void {
    this.tripsService.getTrips().subscribe({
      next: (data: Trip[]) => this.trips = data, // tipado explícito
      error: (err: any) => console.error('Error fetching trips', err)
    });
  }
}
