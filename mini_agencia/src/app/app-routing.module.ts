import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandigComponent } from './pages/landig/landig.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
const routes: Routes = [
  { path: '', component: LandigComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
