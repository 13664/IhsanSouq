import { Component, inject, OnInit} from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { CharityCase } from './shared/models/charityCase';
import { Pagination } from './shared/models/pagination';
import { PortalService } from './core/services/portal.service';
import { PortalComponent } from "./features/portal/portal.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, PortalComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent{

}
