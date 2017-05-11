import { Component, ViewEncapsulation } from '@angular/core';
import './Styles/app.scss';

@Component({
	selector: 'cms-app',
	encapsulation: ViewEncapsulation.None,
	template: `
		<router-outlet></router-outlet>
	`
})
export class AppComponent { }
