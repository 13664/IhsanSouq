import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { PortalComponent } from './features/portal/portal.component';
import { CharityCaseDetailsComponent } from './features/portal/charity-case-details/charity-case-details.component';

export const routes: Routes = [
  {path:'', component: HomeComponent},
  {path:'portal', component: PortalComponent},
  {path:'portal/:id', component: CharityCaseDetailsComponent},
  {path:'**', redirectTo: '', pathMatch: 'full'}



];
