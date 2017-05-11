import { NgModule }               	from '@angular/core';
import { RouterModule, Routes }   	from '@angular/router';
import { PageListComponent }      	from './page-list.component';
import { AddPageComponent }      	from './add-page.component';

const pageRoutes: Routes = [
	{ path: 'admin/pages',  component: PageListComponent },
	{ path: 'admin/pages/new',  component: AddPageComponent }
];

@NgModule({
	imports: [
		RouterModule.forChild(pageRoutes)
	],
	exports: [
		RouterModule
	]
})
export class PageRoutingModule { }
