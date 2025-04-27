import { Component, inject, OnInit } from '@angular/core';
import { PortalComponent } from '../portal.component';
import { ActivatedRoute } from '@angular/router';
import { CharityCase } from '../../../shared/models/charityCase';
import { PortalService } from '../../../core/services/portal.service';
import { CommonModule } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider, MatDividerModule } from '@angular/material/divider';
import {
  Payment,
  PaymentService,
} from '../../../core/services/payment.service';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-charity-case-details',
  imports: [
    CommonModule, // ⬅️ This includes CurrencyPipe, DatePipe, etc.
    FormsModule,
    MatButton,
    MatIcon,
    MatFormField,
    MatInput,
    MatLabel,
    MatDivider,
    MatTableModule, // For Material table
    MatDividerModule, // Optional
    MatProgressBarModule,
  ],
  templateUrl: './charity-case-details.component.html',
  styleUrl: './charity-case-details.component.scss',
})
export class CharityCaseDetailsComponent implements OnInit {
  private portalService = inject(PortalService);
  private paymentService = inject(PaymentService);
  private activatedRoute = inject(ActivatedRoute);
  charityCase?: CharityCase;
  donationAmount: number = 0;
  payments: Payment[] = [];
  displayedColumns: string[] = ['uuid', 'amount', 'status', 'paymentTime'];

  ngOnInit(): void {
    this.loadCharityCase();
  }

  loadCharityCase() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.portalService.getCharityCase(+id).subscribe({
      next: (charityCase) => {
        this.charityCase = charityCase;
        this.loadPayments();
      },
      error: (error) => console.log(error),
    });
  }

  getProgress(): number {
    if (!this.charityCase) return 0;
    const collected = this.charityCase.amountCollected;
    const requested = this.charityCase.amountRequested;
    return requested > 0 ? ((collected /100) / requested) * 100 : 0;
  }

  donate() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;

    const invoiceData = {
      storeId: 6,
      amount: this.donationAmount * 100,
      invoiceId: id,
      returnUrl: 'https://localhost:4200/payment-status',
    };

    this.paymentService.createPaymentInvoice(invoiceData).subscribe({
      next: (response) => {
        sessionStorage.setItem('payment-uuid', response.payment_uuid);
        window.location.href = response.checkout_url;
      },
      error: (error) => {
        console.error('Error creating invoice:', error);
        alert('Failed to process the donation. Please try again.');
      },
    });
  }

  loadPayments() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.paymentService.getPaymentsByCharityCaseId(Number(id)).subscribe({
      next: (data) => (this.payments = data),
      error: (err) => console.error(err),
    });
  }
}
