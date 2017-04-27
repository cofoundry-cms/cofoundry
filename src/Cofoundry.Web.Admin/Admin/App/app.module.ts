import { NgModule }         from '@angular/core';
import { BrowserModule }    from '@angular/platform-browser';
import { FormsModule }      from "@angular/forms";
import { HttpModule }       from "@angular/http";

import { AppComponent }     from './app.component';
import { DashboardModule }  from './modules/dashboard/dashboard.module';
import { SharedModule }     from './modules/shared/shared.module';
import { AppRoutingModule } from './app.routing';

@NgModule({
  imports: [
      BrowserModule,
      FormsModule,
      HttpModule,
      SharedModule.forRoot(),
      AppRoutingModule,
      DashboardModule
  ],
  declarations: [
      AppComponent
  ],
  bootstrap: [
      AppComponent 
  ]
})
export class AppModule { }
