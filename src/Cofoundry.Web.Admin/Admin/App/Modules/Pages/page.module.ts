import { NgModule }                 from '@angular/core';
import { SharedModule }             from '../shared/shared.module';

import { PageListComponent }        from './page-list.component';
import { AddPageComponent }        	from './add-page.component';
import { PageService }              from './page.service';
import { PageRoutingModule }        from './page.routing';

@NgModule({
	imports: [
		SharedModule,
		PageRoutingModule
	],
	declarations: [
		PageListComponent,
		AddPageComponent
	],
	providers: [
		PageService
	]
})
export class PageModule { }
