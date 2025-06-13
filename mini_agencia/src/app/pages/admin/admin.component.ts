import { Component } from '@angular/core';
import { User } from '../../models/user.model';
import { UsersService  } from '../../services/users.service';
import { TripsService  } from '../../services/trips.service';
import { Trip } from '../../models/trip.model';
import { CreateTripRequest } from '../../models/trip-request.model';
@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent {

  users: User[] = [];
  selectedUser?: User;
  message: string = '';
  error: string = '';
    trips: Trip[] = [];

  newTrip: CreateTripRequest = {
    destination: '',
    departureDate: '',
    price: 0,
    isAvailable: true
  };

  editingTripId: string | null = null;

  constructor(private usersService: UsersService,private tripService: TripsService) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadTrips();
  }

  loadUsers() {
    this.usersService.getAllUsers().subscribe({
      next: (data) => {
        this.users = data;
      },
      error: (err) => {
        this.error = 'No se pudieron cargar los usuarios.';
        console.error(err);
      }
    });
  }

    loadTrips() {
    this.tripService.getTrips().subscribe({
      next: (data) => this.trips = data,
      error: (err) => console.error('Error loading trips:', err)
    });
  }

  editUser(user: User) {
    this.selectedUser = { ...user };
    this.message = '';
    this.error = '';
  }

createTrip() {
  // Verifica que hay un valor de fecha
  if (!this.newTrip.departureDate) {
    alert("Please select a valid departure date.");
    return;
  }

  // Formatea la fecha correctamente a ISO (asegúrate que es un string válido para Date)
  const parsedDate = new Date(this.newTrip.departureDate);

  if (isNaN(parsedDate.getTime())) {
    alert("Invalid date format.");
    return;
  }

  const tripToSend = {
    destination: this.newTrip.destination,
    departureDate: parsedDate.toISOString(), // ← formato ISO correcto
    price: this.newTrip.price,
    isAvailable: this.newTrip.isAvailable
  };

  this.tripService.createTrip(tripToSend).subscribe({
    next: () => {
      this.newTrip = { destination: '', departureDate: '', price: 0, isAvailable: true };
      this.loadTrips();
    },
    error: (err) => {
      console.error('Error creating trip:', err);
    }
  });
}

  updateTrip(trip: Trip) {
    const { id, ...rest } = trip;
    this.tripService.updateTrip(id, rest).subscribe(() => {
      this.editingTripId = null;
      this.loadTrips();
    });
  }
  saveUser() {
    if (this.selectedUser) {
      this.usersService.updateUser(this.selectedUser).subscribe({
        next: () => {
          this.message = 'Usuario actualizado con éxito.';
          this.selectedUser = undefined;
          this.loadUsers();
        },
        error: (err) => {
          this.error = 'Error al actualizar el usuario.';
          console.error(err);
        }
      });
    }
  }

  deleteTrip(id: string) {
    if (confirm('¿Eliminar este destino?')) {
      this.tripService.deleteTrip(id).subscribe(() => this.loadTrips());
    }
  }

  cancelEdit() {
    this.selectedUser = undefined;
  }

  deleteUser(user: User) {
  if (confirm(`¿Estás seguro de eliminar al usuario "${user.name}"?`)) {
    this.usersService.deleteUser(user.id).subscribe({
      next: (res) => {
        this.message = res.message || 'Usuario eliminado exitosamente.';
        this.loadUsers();
      },
      error: (err) => {
        this.error = 'Error al eliminar usuario.';
        console.error(err);
      }
    });
  }
}


}
