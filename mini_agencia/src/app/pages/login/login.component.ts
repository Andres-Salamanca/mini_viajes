import { Component } from '@angular/core';
import { UsersService  } from '../../services/users.service';
import { LoginRequest } from '../../models/auth.models';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';
  constructor(private authService: UsersService, private router: Router) {}

  login() {
    const payload: LoginRequest = {
      username: this.username,
      password: this.password
    };

    this.authService.login(payload).subscribe({
      next: (res) => {
        console.log('Login successful:', res);
        localStorage.setItem('token', res.token);
       const role = res.user.role;
        if (role === 'Admin') {
          this.router.navigate(['/admin']);
        } else {
          this.router.navigate(['/user']);
        }
      },
      error: (err) => {
        this.errorMessage = err.error;
        console.error('Login error:', err);
      }
    });
  }
}
