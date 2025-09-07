import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {
  city = '';
  country = '';
  guests = 1;

  constructor(private router: Router) {}

  search() {
    const queryParams: any = {};
    if (this.city) queryParams.city = this.city;
    if (this.country) queryParams.country = this.country;
    if (this.guests) queryParams.guests = this.guests;
    this.router.navigate(['/properties'], { queryParams });
  }
}
