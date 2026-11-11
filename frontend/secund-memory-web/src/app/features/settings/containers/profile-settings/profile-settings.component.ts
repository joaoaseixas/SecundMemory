import { Component, inject } from '@angular/core';
import { AuthStore } from '../../../../core/auth/auth.store';

@Component({
  selector: 'app-profile-settings',
  standalone: true,
  templateUrl: './profile-settings.component.html',
  styleUrl: './profile-settings.component.scss',
})
export class ProfileSettingsComponent {
  protected readonly authStore = inject(AuthStore);
}
