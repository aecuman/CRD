import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AllCommunityModule, ModuleRegistry } from 'ag-grid-community'; 
import {NgxCurrencyDirective} from 'ngx-currency';

import { AgGridAngular } from "ag-grid-angular";

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { PortalComponent } from './portal/portal.component';
import {API_BASE_URL} from './api.service';
import { environment } from 'src/environment/environment';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ErrorInterceptor } from './_helpers/error.interceptor';
import { StatusComponent } from './status/status.component';
import { PlantsComponent } from './plants/plants.component';
import { StructuresComponent } from './structures/structures.component';
import { SettingsComponent } from './settings/settings.component';
import { PageListViewComponent } from './_common/page-list-view/page-list-view.component';
import { MultiSelectComponent } from './_common/multi-select/multi-select.component';
import { FilterByNamePipe, FilterRateTablePipe, ListFilterPipe } from './app.pipe';
import { ClickOutsideDirective } from './_common/multi-select/click-outisde.directive';
import { ReviewsComponent } from './reviews/reviews.component';
import { WorkflowManagementComponent } from './workflow-management/workflow-management.component';
import { WorkflowStepManagementComponent } from './workflow-step-management/workflow-step-management.component';
import { DistrictRatesComponent } from './district-rates/district-rates.component';
import { PlantRateMatrixComponent } from './matrix/plant-rate-matrix/plant-rate-matrix.component';
import { PlantRatesComponent } from './plant-rates/plant-rates.component';
import { StructureRatesComponent } from './structure-rates/structure-rates.component';
import { StructureRateMatrixComponent } from './matrix/structure-rate-matrix/structure-rate-matrix.component';
import { ModerationComponent } from './moderation/moderation.component';
import { ModerationPopupComponent } from './moderation-popup/moderation-popup.component';
import { PlantsModerationComponent } from './moderation/plants-moderation/plants-moderation.component';
import { StructuresModerationComponent } from './moderation/structures-moderation/structures-moderation.component';
import { ModerationReportComponent } from './moderation-report/moderation-report.component';
import { FinalReportComponent } from './final-report/final-report.component';
import { FinalReportExportComponent } from './final-report-export/final-report-export.component';
import { PublishedComponent } from './published/published.component';
import { UsersComponent } from './users/users.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { DistrictWorkflowDetailComponent } from './district-workflow-detail/district-workflow-detail.component';
import { DistrictRateCompareModalComponent } from './district-rate-compare-modal/district-rate-compare-modal.component';
import { CrdMapComponent } from './crd-map/crd-map.component';
import { PublishedDistrictComponent } from './published-district/published-district.component';
import { ManualsComponent } from './manuals/manuals.component';
import { RateTemplateComponent } from './rate-template/rate-template.component';
import { AuthGuard } from './authguard';
ModuleRegistry.registerModules([AllCommunityModule]);

const routes: Routes = [
  { path: "", redirectTo: "portal", pathMatch: "full" },

  {
    path: "portal", component: PortalComponent,canActivate:[AuthGuard], children: [
      { path: "", redirectTo: "map", pathMatch: "full" },
      { path: 'map', component: CrdMapComponent },
      { path: 'status', component: StatusComponent },
      { path: 'published', component: PublishedComponent },
      { path: 'plants', component: PlantsComponent },
      { path: 'structures', component: StructuresComponent },
      { path: 'settings', component: SettingsComponent },
      { path: 'reviews', component: ReviewsComponent },
      { path: 'rates/vegetation/:id', component: PlantRatesComponent },
      { path: 'rates/structures/:id', component: StructureRatesComponent },
      { path: 'rates/moderation/vegetation/:id', component: PlantsModerationComponent },
      { path: 'rates/moderation/structures/:id', component: StructuresModerationComponent },
      { path: 'rates/moderation/report/:id', component: ModerationReportComponent },
      { path: 'rates/published/:id', component: PublishedDistrictComponent },
      { path: 'rates/:id', component: DistrictRatesComponent },
      { path: 'users', component: UsersComponent },
      { path: 'district/:districtId/:districtRateId/workflow-status', component: DistrictWorkflowDetailComponent },
      { path: 'templates', component: RateTemplateComponent },
      { path: 'manuals', component: ManualsComponent }


    ] },
  { path: 'final-report-download', component: FinalReportExportComponent, outlet: 'print' },
  { path: 'login', component: LoginComponent },
  { path: 'reset-password', component: ResetPasswordComponent }

]
      
@NgModule({ declarations: [
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
        FilterByNamePipe,
        FilterRateTablePipe,
        ClickOutsideDirective,
        ReviewsComponent,
        WorkflowManagementComponent,
        WorkflowStepManagementComponent,
        DistrictRatesComponent,
        PlantRateMatrixComponent,
        PlantRatesComponent,
        StructureRatesComponent,
        StructureRateMatrixComponent,
        ModerationComponent,
        ModerationPopupComponent,
        PlantsModerationComponent,
        StructuresModerationComponent,
        ModerationReportComponent,
        FinalReportComponent,
        FinalReportExportComponent,
        PublishedComponent,
        UsersComponent,
        ResetPasswordComponent,
        DistrictWorkflowDetailComponent,
        DistrictRateCompareModalComponent,
        PublishedComponent,
        CrdMapComponent,
        PublishedDistrictComponent,
        ManualsComponent,
        RateTemplateComponent
    ],
    bootstrap: [AppComponent], 
    imports: [BrowserModule,
        ReactiveFormsModule,
        FormsModule,
        RouterModule.forRoot(routes),
        AgGridAngular,
        NgxCurrencyDirective
      ]
        , 
        providers: [
        { provide: API_BASE_URL, useValue: environment.baseUrl },
        provideHttpClient(withInterceptorsFromDi()),
    ] })
export class AppModule { }
