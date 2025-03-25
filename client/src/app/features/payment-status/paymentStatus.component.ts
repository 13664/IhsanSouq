import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PaymentService } from '../../core/services/payment.service';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-payment-status',
  templateUrl: './paymentStatus.component.html',
  styleUrls: ['./paymentStatus.component.scss'],
  imports: [CommonModule, MatIcon, MatButton],
})
export class PaymentStatusComponent implements OnInit {
  private activatedRoute = inject(ActivatedRoute);
  private paymentService = inject(PaymentService);

  paymentStatus: 'success' | 'error' | 'fail' | null = null;
  message: string = '';

  ngOnInit(): void {
    this.checkPaymentStatus();
  }

  checkPaymentStatus(): void {
    const uuid = sessionStorage.getItem('payment-uuid');
    if (!uuid) {
      this.paymentStatus = 'error';
      this.message = 'Invalid payment identifier.';
      return;
    }

    this.paymentService.getPaymentStatus(uuid).subscribe({
      next: (response) => {
        if (response.status === 'success') {
          this.paymentStatus = 'success';
          this.message = 'Your payment was successful. Thank you!';
        } else if (response.status === 'fail') {
          this.paymentStatus = 'fail';
          this.message = 'Your payment failed. Please try again.';
        } else {
          this.paymentStatus = 'error';
          this.message = 'An error occurred while processing your payment.';
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
