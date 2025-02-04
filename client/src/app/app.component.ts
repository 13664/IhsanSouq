import { Component, inject, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { CharityCase } from './shared/models/charityCase';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  baseUrl = 'https://localhost:5001/api/'
  private http = inject(HttpClient);
  title = 'IhsanSouq';
  charityCases: CharityCase[] =[];

  ngOnInit(): void {
    this.http.get<Pagination<CharityCase>>(this.baseUrl + 'charitycases').subscribe({
      next: response => this.charityCases =response.data,
      error: error => console.log(error),
      complete: () => console.log('complete')

      
    })
  }


}
