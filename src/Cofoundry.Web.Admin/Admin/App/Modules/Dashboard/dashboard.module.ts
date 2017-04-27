import { NgModule }                 from '@angular/core';
import { SharedModule }             from '../shared/shared.module';

import { DashboardComponent }       from './dashboard.component';
import { DashboardListComponent }   from './dashboard-list.component';
import { DashboardService }         from './dashboard.service';
import { DashboardRoutingModule }   from './dashboard.routing';

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
        DashboardService 
    ]
})
export class DashboardModule { }