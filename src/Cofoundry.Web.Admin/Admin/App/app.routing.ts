import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

export const appRoutes: Routes = [
	{ path: '', redirectTo: 'admin/dashboard', pathMatch: 'full'}
];

@NgModule({
	imports: [
		RouterModule.forRoot(
			appRoutes
		)
	],
	exports: [
		RouterModule
	],
	providers: [

	]
})
export class AppRoutingModule { }
