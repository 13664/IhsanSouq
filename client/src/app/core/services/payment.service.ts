import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient);

  getPaymentAuthToken(): Observable<{ token: string }> {
    return this.http.get<{ token: string }>(
      `${this.baseUrl}Multicard/auth-token`
    );
  }

  createPaymentInvoice(
    invoiceData: {
      store_id: number;
      amount: number;
      invoice_id: string;
      return_url: string;
      callback_url: string;
    },
    token: string
  ): Observable<{ uuid: string }> {
    return this.http.post<{ uuid: string }>(
      `${this.baseUrl}Payment/create-invoice`,
      invoiceData,
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
  }

  getPaymentStatus(invoiceId: string): Observable<any> {
    return this.http.get(`https://dev-mesh.multicard.uz/payment/${invoiceId}`);
  }
}
