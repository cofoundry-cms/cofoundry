import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

// The app module
import { AppModule } from './app.module';

if (process.env.ENV === 'production') {
	enableProdMode();
}

// Compile and launch the module
platformBrowserDynamic().bootstrapModule(AppModule);
