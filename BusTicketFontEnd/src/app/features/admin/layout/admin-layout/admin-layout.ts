import { CommonModule } from "@angular/common";
import { Component, signal } from "@angular/core";
import { RouterOutlet, RouterLink, RouterLinkActive } from "@angular/router";
import { AuthService } from "../../../../core/services/auth.service";


@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.scss'
 
})
export class AdminLayout {
  sidebarCollapsed = signal(false);
  constructor(public auth: AuthService) {}
}
