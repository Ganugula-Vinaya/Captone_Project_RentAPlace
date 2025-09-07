import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { PropertyList } from './pages/property-list/property-list';
import { PropertyDetail } from './pages/property-detail/property-detail';
import { OwnerDashboard } from './pages/owner/owner-dashboard/owner-dashboard';
import { AddProperty } from './pages/owner/add-property/add-property';
import { AuthGuard } from './core/guards/auth.guard';
import { OwnerEdit } from './pages/owner/owner-edit/owner-edit';



export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'properties', component: PropertyList },
  { path: 'properties/:id', component: PropertyDetail },
  { path: 'owner', component: OwnerDashboard, canActivate: [AuthGuard] },
  { path: 'owner/add', component: AddProperty, canActivate: [AuthGuard] },
  { path: 'owner/edit/:id', component: OwnerEdit, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '' }
];
