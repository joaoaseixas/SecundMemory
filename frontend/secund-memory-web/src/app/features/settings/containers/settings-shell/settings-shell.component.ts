import { Component } from '@angular/core';
import { ProfileSettingsComponent } from '../profile-settings/profile-settings.component';
import { AppearanceSettingsComponent } from '../appearance-settings/appearance-settings.component';
import { ShortcutsSettingsComponent } from '../shortcuts-settings/shortcuts-settings.component';

@Component({
  selector: 'app-settings-shell',
  standalone: true,
  imports: [ProfileSettingsComponent, AppearanceSettingsComponent, ShortcutsSettingsComponent],
  templateUrl: './settings-shell.component.html',
  styleUrl: './settings-shell.component.scss',
})
export class SettingsShellComponent {}
