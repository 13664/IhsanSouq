import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { CharityCase } from '../../shared/models/charityCase';
import { response } from 'express';
import { PortalParams } from '../../shared/models/portalParams';

@Injectable({
  providedIn: 'root'
})
export class PortalService {
  baseUrl = 'https://localhost:5001/api/'
  private http = inject(HttpClient);
  categories: string[] = [];
  urgencyLevels: string[] = [];


  getCharityCases(portalParams: PortalParams){
    let params = new HttpParams();
    if (portalParams.categories.length > 0){
      params = params.append('categories', portalParams.categories.join(','));
    }
    if (portalParams.urgencyLevels.length > 0){
      params = params.append('urgencyLevels', portalParams.urgencyLevels.join(','));
    }
    if(portalParams.sort){
      params = params.append('sort', portalParams.sort);
    }
    if(portalParams.search){
      params = params.append('search', portalParams.search);
    }
    params = params.append('pageSize', portalParams.pageSize);
    params = params.append('pageIndex', portalParams.pageNumber);

    return this.http.get<Pagination<CharityCase>>(this.baseUrl + 'charitycases', {params});
  }
  

  getCategories(){
    if(this.categories.length > 0) return;
    return this.http.get<string[]>(this.baseUrl+'charityCases/categories').subscribe({
      next: response => this.categories = response
    })
  }

  getUrgencyLevels(){
    if(this.urgencyLevels.length > 0) return;
    return this.http.get<string[]>(this.baseUrl+'charityCases/urgencyLevels').subscribe({
      next: response => this.urgencyLevels = response
    })
  }
}
