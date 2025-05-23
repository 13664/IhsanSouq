import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Payment {
  id: number;
  uuid: string;
  charityCaseId: number;
  amount: number;
  status: string;
  paymentTime: string;
  cardPan: string;
}

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  baseUrl = 'https://localhost:5001/api';
  private http = inject(HttpClient);

  createPaymentInvoice(invoiceData: {
    storeId: number;
    amount: number;
    invoiceId: string;
    returnUrl: string;
  }): Observable<{ checkout_url: string; payment_uuid: string }> {
    return this.http.post<{ checkout_url: string; payment_uuid: string }>(
      `${this.baseUrl}/payment/create`,
      invoiceData
    );
  }

  getPaymentStatus(invoiceId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/payment/status/${invoiceId}`);
  }

  getPaymentsByCharityCaseId(charityCaseId: number) {
    return this.http.get<Payment[]>(
      `${this.baseUrl}/payment/case/${charityCaseId}`
    );
  }
}
