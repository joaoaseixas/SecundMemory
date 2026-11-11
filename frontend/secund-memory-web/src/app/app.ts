import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastComponent } from './shared/ui/toast/toast.component';
import { CommandPaletteComponent } from './shared/ui/command-palette/command-palette.component';
import { ThemeService } from './core/services/theme.service';

@Component({
  imports: [RouterOutlet, ToastComponent, CommandPaletteComponent],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  // Injected (not just provided) so it constructs now, before first paint,
  // and stamps data-theme onto <html> — otherwise the theme CSS variables
  // are undefined until something else happens to inject this service.
  private readonly theme = inject(ThemeService);
}
