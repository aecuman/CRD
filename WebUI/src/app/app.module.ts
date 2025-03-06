import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { PortalComponent } from './portal/portal.component';
import {API_BASE_URL} from './api.service';
import { environment } from 'src/environment/environment';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ErrorInterceptor } from './_helpers/error.interceptor';
import { StatusComponent } from './status/status.component';
import { PlantsComponent } from './plants/plants.component';
import { StructuresComponent } from './structures/structures.component';
import { SettingsComponent } from './settings/settings.component';
import { PageListViewComponent } from './_common/page-list-view/page-list-view.component';
import { MultiSelectComponent } from './_common/multi-select/multi-select.component';
import { ListFilterPipe } from './app.pipe';
import { ClickOutsideDirective } from './_common/multi-select/click-outisde.directive';
import { ReviewsComponent } from './reviews/reviews.component';
import { WorkflowManagementComponent } from './workflow-management/workflow-management.component';
import { WorkflowStepManagementComponent } from './workflow-step-management/workflow-step-management.component';

const routes: Routes = [
  { path: "", redirectTo: "portal", pathMatch: "full" },
  { path: "portal", component: PortalComponent,children:[
    { path: "", redirectTo: "status", pathMatch: "full" },
    { path: 'status', component: StatusComponent },
    {path:'plants',component:PlantsComponent},
    {path:'structures',component:StructuresComponent},
    {path:'settings',component:SettingsComponent}
  ] },
  { path: 'login', component: LoginComponent }

]
      
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    PortalComponent,
    StatusComponent,
    PlantsComponent,
    StructuresComponent,
    SettingsComponent,
    PageListViewComponent,
    MultiSelectComponent,
  ListFilterPipe,
  ClickOutsideDirective,
  ReviewsComponent,
  WorkflowManagementComponent,
  WorkflowStepManagementComponent
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(routes)
  ],
  providers: [
    { provide:API_BASE_URL, useValue: environment.baseUrl},
  //  { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
