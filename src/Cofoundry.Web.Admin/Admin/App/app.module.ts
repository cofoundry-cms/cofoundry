import { NgModule }         from '@angular/core';
import { BrowserModule }    from '@angular/platform-browser';
import { FormsModule }      from '@angular/forms';
import { HttpModule }       from '@angular/http';
import { NgbModule } 		from '@ng-bootstrap/ng-bootstrap';

import { AppComponent }     from './app.component';
import { DashboardModule }  from './modules/dashboard/dashboard.module';
import { PageModule }       from './modules/pages/page.module';
import { SharedModule }     from './modules/shared/shared.module';
import { AppRoutingModule } from './app.routing';

@NgModule({
	imports: [
		BrowserModule,
		FormsModule,
		HttpModule,
		NgbModule.forRoot(),
		SharedModule.forRoot(),
		AppRoutingModule,
		DashboardModule,
		PageModule
	],
	declarations: [
		AppComponent
	],
	bootstrap: [
		AppComponent
	]
})
export class AppModule { }
