import { Component } from '@angular/core';
import { UsersService  } from '../../services/users.service';
import { RegisterRequest } from '../../models/auth.models';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  username: string = '';
  email: string = '';
  password: string = '';
  message: string = '';
  errorMessage: string = '';
    constructor(private authService: UsersService, private router: Router) {}

  register() {
    const payload: RegisterRequest = {
      username: this.username,
      email: this.email,
      password: this.password
    };

    this.authService.register(payload).subscribe({
      next: (_) => {
        this.message = 'Registro exitoso, ahora puedes iniciar sesión.';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1000);
      },
      error: (err) => {
        this.errorMessage = err.error;
        this.message = '';
        console.error('Register error:', err);
      }
    });
  }
}
