import { CommonModule } from "@angular/common";
import { Component, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterLink, Router } from "@angular/router";
import { AuthService } from "../../../core/services/auth.service";


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
  
})
export class Login {
  form     = { email: '', password: '' };
  loading  = signal(false);
  error    = signal('');
  showPass = signal(false);

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.loading.set(true);
    this.error.set('');
    this.auth.login(this.form).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.isSuccess) {
          this.router.navigate([res.user?.role === 'Admin' ? '/admin/dashboard' : '/']);
        } else {
          this.error.set(res.message || 'Login failed');
        }
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Invalid email or password. Please try again.');
      }
    });
  }
}
