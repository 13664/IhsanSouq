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

    this.paymentService.getPaymentAuthToken().subscribe({
      next: (response) => {
        const token = response.token;

        const invoiceData = {
          store_id: Number(id),
          amount: this.donationAmount * 100,
          invoice_id: 'INV-20250323-002',
          return_url: 'https://localhost:4200/payment-status',
          callback_url: 'https://yourwebsite.com/payment-callback',
        };

        this.paymentService.createPaymentInvoice(invoiceData, token).subscribe({
          next: (response) => {
            const uuidSplit = response.uuid.split('/');
            sessionStorage.setItem(
              'payment-uuid',
              uuidSplit[uuidSplit.length - 1]
            );
            window.location.href = response.uuid;
          },
          error: (error) => {
            console.error('Error creating invoice:', error);
            alert('Failed to process the donation. Please try again.');
          },
        });
      },
      error: (error) => {
        console.error('Error fetching auth token:', error);
        alert('Failed to authenticate. Please try again.');
      },
    });
  }
}
