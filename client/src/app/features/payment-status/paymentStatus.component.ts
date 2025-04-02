import { Component, OnInit, inject } from '@angular/core';
import { PaymentService } from '../../core/services/payment.service';
import { CommonModule } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { SafeUrlPipe } from '../../core/utils/safe-url.pipe';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-payment-status',
  templateUrl: './paymentStatus.component.html',
  styleUrls: ['./paymentStatus.component.scss'],
  imports: [CommonModule, MatButton, SafeUrlPipe, RouterModule],
})
export class PaymentStatusComponent implements OnInit {
  private paymentService = inject(PaymentService);

  paymentStatus: 'success' | 'error' | 'draft' | null = null;
  uuid: string | null = null;
  checkoutUrl: string = '';
  message: string = '';

  ngOnInit(): void {
    this.uuid = sessionStorage.getItem('payment-uuid');

    if (!this.uuid) {
      this.paymentStatus = 'error';
      this.message = 'Invalid payment identifier.';
      return;
    }

    this.checkoutUrl =
      'https://localhost:5001/api/payment/receipt-proxy/' + this.uuid;

    this.checkPaymentStatus();
  }

  checkPaymentStatus(): void {
    if (!this.uuid) {
      this.paymentStatus = 'error';
      this.message = 'Invalid payment identifier.';
      return;
    }

    this.paymentService.getPaymentStatus(this.uuid).subscribe({
      next: (response) => {
        if (response.data.status === 'success') {
          this.paymentStatus = 'success';
          this.message = 'Your payment was successful. Thank you!';
        } else if (response.data.status === 'draft') {
          this.paymentStatus = 'draft';
          this.message = 'Your payment is pending. Please make payment.';
        } else {
          this.paymentStatus = 'error';
          this.message = 'Your payment failed. Please try again.';
        }
      },
      error: (error) => {
        console.error('Error fetching payment status:', error);
        this.paymentStatus = 'error';
        this.message = 'An error occurred while checking the payment status.';
      },
    });
  }
}
