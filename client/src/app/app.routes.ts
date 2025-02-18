import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { PortalComponent } from './features/portal/portal.component';
import { CharityCaseDetailsComponent } from './features/portal/charity-case-details/charity-case-details.component';
import { TestErrorComponent } from './features/test-error/test-error.component';
import { NotFoundComponent } from './shared/component/not-found/not-found.component';
import { ServerErrorComponent } from './shared/component/server-error/server-error.component';
import { LoginComponent } from './features/account/login/login.component';
import { RegisterComponent } from './features/account/register/register.component';

export const routes: Routes = [
  {path:'', component: HomeComponent},
  {path:'portal', component: PortalComponent},
  {path:'portal/:id', component: CharityCaseDetailsComponent},
  {path:'account/login', component: LoginComponent},
  {path:'account/register', component: RegisterComponent},
  {path:'test-error', component: TestErrorComponent},
  {path:'not-found', component: NotFoundComponent},
  {path:'server-error', component: ServerErrorComponent},
  {path:'**', redirectTo: 'not-found', pathMatch: 'full'}



];
