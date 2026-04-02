import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from "@angular/router";
import { AuthService } from "./core/services/auth.service";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App{
  year = new Date().getFullYear();
  constructor(public auth: AuthService, private router: Router) {}
  logout() { this.auth.logout(); }
}
