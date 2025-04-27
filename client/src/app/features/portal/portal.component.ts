import { Component, inject, OnInit } from '@angular/core';
import { PortalService } from '../../core/services/portal.service';
import { CharityCase } from '../../shared/models/charityCase';
import { MatCard } from '@angular/material/card';
import { CharityCaseItemComponent } from './charity-case-item/charity-case-item.component';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import {
  MatList,
  MatListOption,
  MatSelectionList,
  MatSelectionListChange,
} from '@angular/material/list';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatPaginator, PageEvent } from '@angular/material/paginator';

import { PortalParams } from '../../shared/models/portalParams';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-portal',
  imports: [
    MatCard,
    CommonModule,
    CharityCaseItemComponent,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
    MatPaginator,
    FormsModule,
  ],
  templateUrl: './portal.component.html',
  styleUrl: './portal.component.scss',
})
export class PortalComponent implements OnInit {
  private portalService = inject(PortalService);
  private dialogService = inject(MatDialog);
  charityCases?: Pagination<CharityCase>;
  private accountService = inject(AccountService);
  private router = inject(Router);

  isAdmin = this.accountService.isAdmin;

  sortOptions = [
    { name: 'Date:Old-New', value: 'dateAsc' },
    { name: 'Date:New-Old', value: 'dateDesc' },
  ];
  portalParams = new PortalParams();
  pageSizeOptions = [5, 10, 15, 20];

  ngOnInit(): void {
    this.InitilizePortal();
  }

  InitilizePortal() {
    this.portalService.getCategories();
    this.portalService.getUrgencyLevels();
    this.getCharityCases();
  }
  getCharityCases() {
    this.portalService.getCharityCases(this.portalParams).subscribe({
      next: (response) => (this.charityCases = response),
      error: (error) => console.log(error),
    });
  }
  onSearchChange() {
    this.portalParams.pageNumber = 1;
    this.getCharityCases();
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.portalParams.sort = selectedOption.value;
      this.portalParams.pageNumber = 1;
      this.getCharityCases();
    }
  }
  handlePageEvent(event: PageEvent) {
    this.portalParams.pageNumber = event.pageIndex + 1;
    this.portalParams.pageSize = event.pageSize;
    this.getCharityCases();
  }
  openFilterDialog() {
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        selectedCategories: this.portalParams.categories,
        selectedUrgencyLevels: this.portalParams.urgencyLevels,
      },
    });
    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          this.portalParams.categories = result.selectedCategories;
          this.portalParams.urgencyLevels = result.selectedUrgencyLevels;
          this.portalParams.pageNumber = 1;

          this.getCharityCases();
        }
      },
    });
  }

  onCreate() {
    this.router.navigate(['/portal/create']);
  }
}
