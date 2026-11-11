import { Component, inject } from '@angular/core';
import { ButtonComponent } from '../../../../shared/ui/button/button.component';
import { ThemeService } from '../../../../core/services/theme.service';

@Component({
  selector: 'app-appearance-settings',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './appearance-settings.component.html',
  styleUrl: './appearance-settings.component.scss',
})
export class AppearanceSettingsComponent {
  protected readonly theme = inject(ThemeService);
}
