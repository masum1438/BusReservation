// import { Component } from '@angular/core';

import { CommonModule } from "@angular/common";
import { Component, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterLink, Router } from "@angular/router";
import { AuthService } from "../../../core/services/auth.service";


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register{
  form     = { username: '', email: '', password: '', fullName: '', mobileNumber: '' };
  loading  = signal(false);
  error    = signal('');
  showPass = signal(false);
  step     = signal(1);

  get pwStrength(): () => number {
    return () => {
      const p = this.form.password;
      if (!p) return 0;
      let s = 0;
      if (p.length >= 8)  s += 25;
      if (p.length >= 12) s += 25;
      if (/[A-Z]/.test(p)) s += 25;
      if (/[0-9!@#$%^&*]/.test(p)) s += 25;
      return s;
    };
  }

  constructor(private auth: AuthService, private router: Router) {}

  nextStep() { this.step.set(2); }

  submit() {
    this.loading.set(true);
    this.error.set('');
    this.auth.register(this.form).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.isSuccess) { this.router.navigate(['/']); }
        else { this.error.set(res.message || 'Registration failed'); }
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Registration failed. Username or email may already exist.');
      }
    });
  }
}
