import { Component, inject, Input } from '@angular/core';
import { CharityCase } from '../../../shared/models/charityCase';
import {
  MatCard,
  MatCardActions,
  MatCardContent,
} from '@angular/material/card';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';

@Component({
  selector: 'app-charity-case-item',
  imports: [
    MatCard,
    MatCardContent,
    CurrencyPipe,
    MatCardActions,
    MatButton,
    MatIcon,
    RouterLink,
    CommonModule,
  ],
  templateUrl: './charity-case-item.component.html',
  styleUrl: './charity-case-item.component.scss',
})
export class CharityCaseItemComponent {
  @Input() charityCase?: CharityCase;
  private accountService = inject(AccountService);
  private router = inject(Router);

  isAdmin = this.accountService.isAdmin;

  onEdit(id: number) {
    this.router.navigate(['/portal/edit', id]);
  }
}
