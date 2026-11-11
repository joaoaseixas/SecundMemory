import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/auth/auth.service';
import { ButtonComponent } from '../../../../shared/ui/button/button.component';
import { InputComponent } from '../../../../shared/ui/input/input.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ButtonComponent, InputComponent, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly email = signal('');
  readonly displayName = signal('');
  readonly password = signal('');
  readonly error = signal<string | null>(null);
  readonly isSubmitting = signal(false);

  submit(): void {
    this.error.set(null);
    this.isSubmitting.set(true);

    this.authService.register(this.email(), this.displayName(), this.password()).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => {
        this.error.set('Could not create an account with these details.');
        this.isSubmitting.set(false);
      },
    });
  }
}
