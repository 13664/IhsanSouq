import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { CharityCase } from '../../shared/models/charityCase';
import { PortalParams } from '../../shared/models/portalParams';

@Injectable({
  providedIn: 'root',
})
export class PortalService {
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient);
  categories: string[] = [];
  urgencyLevels: string[] = [];

  token = localStorage.getItem('token');
  headers = this.token
    ? new HttpHeaders().set('Authorization', `Bearer ${this.token}`)
    : new HttpHeaders();

  getCharityCase(id: number) {
    return this.http.get<CharityCase>(this.baseUrl + 'charityCases/' + id);
  }

  getCharityCases(portalParams: PortalParams) {
    let params = new HttpParams();
    if (portalParams.categories.length > 0) {
      params = params.append('categories', portalParams.categories.join(','));
    }
    if (portalParams.urgencyLevels.length > 0) {
      params = params.append(
        'urgencyLevels',
        portalParams.urgencyLevels.join(',')
      );
    }
    if (portalParams.sort) {
      params = params.append('sort', portalParams.sort);
    }
    if (portalParams.search) {
      params = params.append('search', portalParams.search);
    }
    params = params.append('pageSize', portalParams.pageSize);
    params = params.append('pageIndex', portalParams.pageNumber);

    return this.http.get<Pagination<CharityCase>>(
      this.baseUrl + 'charitycases',
      { params }
    );
  }

  getCategories() {
    if (this.categories.length > 0) return;
    return this.http
      .get<string[]>(this.baseUrl + 'charityCases/categories')
      .subscribe({
        next: (response) => (this.categories = response),
      });
  }

  getUrgencyLevels() {
    if (this.urgencyLevels.length > 0) return;
    return this.http
      .get<string[]>(this.baseUrl + 'charityCases/urgencyLevels')
      .subscribe({
        next: (response) => (this.urgencyLevels = response),
      });
  }

  createCharityCase(charityCase: any) {
    return this.http.post(`${this.baseUrl}charitycases`, charityCase, {
      headers: this.headers,
    });
  }

  updateCharityCase(id: number, charityCase: any) {
    return this.http.put(`${this.baseUrl}charitycases/${id}`, charityCase, {
      headers: this.headers,
    });
  }
}
