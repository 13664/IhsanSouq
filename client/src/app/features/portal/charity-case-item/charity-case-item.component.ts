import { Component, Input } from '@angular/core';
import { CharityCase } from '../../../shared/models/charityCase';
import { MatCard, MatCardActions, MatCardContent } from '@angular/material/card';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-charity-case-item',
  imports: [MatCard, 
    MatCardContent, CurrencyPipe, MatCardActions, MatButton, MatIcon, RouterLink
  ],
  templateUrl: './charity-case-item.component.html',
  styleUrl: './charity-case-item.component.scss'
})
export class CharityCaseItemComponent {
  @Input() charityCase?: CharityCase;
}
