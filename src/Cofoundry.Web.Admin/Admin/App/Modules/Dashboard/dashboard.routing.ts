import { NgModule }             from '@angular/core';
import { RouterModule, Routes }  from '@angular/router';
import { DashboardListComponent } from './dashboard-list.component';

const dashboardRoutes: Routes = [
  { path: 'admin/dashboard',  component: DashboardListComponent }
];

@NgModule({
  imports: [
    RouterModule.forChild(dashboardRoutes)
  ],
  exports: [
    RouterModule
  ]
})
export class DashboardRoutingModule { }