import { Component, inject, OnInit } from '@angular/core';
import { PortalComponent } from '../portal.component';
import { ActivatedRoute } from '@angular/router';
import { CharityCase } from '../../../shared/models/charityCase';
import { PortalService } from '../../../core/services/portal.service';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput} from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';



@Component({
  selector: 'app-charity-case-details',
  imports: [CurrencyPipe, MatButton, MatIcon, MatFormField, MatInput, MatLabel, MatDivider ],
  templateUrl: './charity-case-details.component.html',
  styleUrl: './charity-case-details.component.scss'
})
export class CharityCaseDetailsComponent implements OnInit {
  private portalService = inject(PortalService);
  private activatedRoute = inject(ActivatedRoute);
  charityCase?: CharityCase;

  ngOnInit(): void {
    this.loadCharityCase();
  }

  loadCharityCase(){
    const id =this.activatedRoute.snapshot.paramMap.get('id');
    if(!id) return;
    this.portalService.getCharityCase(+id).subscribe({
      next: charityCase => this.charityCase = charityCase,
      error: error => console.log(error)
      
      
    })
  }

}
