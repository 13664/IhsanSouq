import { Component, inject, OnInit } from '@angular/core';
import { PortalComponent } from '../portal.component';
import { ActivatedRoute } from '@angular/router';
import { CharityCase } from '../../../shared/models/charityCase';
import { PortalService } from '../../../core/services/portal.service';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';
import { PaymentService } from '../../../core/services/payment.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-charity-case-details',
  imports: [
    CurrencyPipe,
    MatButton,
    MatIcon,
    MatFormField,
    MatInput,
    MatLabel,
    MatDivider,
    FormsModule,
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

  ngOnInit(): void {
    this.loadCharityCase();
  }

  loadCharityCase() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.portalService.getCharityCase(+id).subscribe({
      next: (charityCase) => (this.charityCase = charityCase),
      error: (error) => console.log(error),
    });
  }

  donate() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;

    const invoiceData = {
      storeId: Number(id),
      amount: this.donationAmount * 100,
      invoiceId: 'INV-20250323-002',
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
}
