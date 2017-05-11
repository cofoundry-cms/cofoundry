import { NgModule }                 from '@angular/core';
import { SharedModule }             from '../shared/shared.module';

import { DashboardComponent }       from './dashboard.component';
import { DashboardListComponent }   from './dashboard-list.component';
import { DashboardService }         from './dashboard.service';
import { DashboardRoutingModule }   from './dashboard.routing';
import { NgbActiveModal } 			from '@ng-bootstrap/ng-bootstrap';

@NgModule({
	imports: [
		SharedModule,
		DashboardRoutingModule
	],
	declarations: [
		DashboardListComponent,
		DashboardComponent
	],
	providers: [
		DashboardService,
		NgbActiveModal
	],
	entryComponents: [
		DashboardComponent
	]
})
export class DashboardModule { }
