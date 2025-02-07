import { Component, inject } from '@angular/core';
import { PortalService } from '../../../core/services/portal.service';
import {MatDivider} from '@angular/material/divider';
import {MatListOption, MatSelectionList} from '@angular/material/list';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-filters-dialog',
  imports: [
    MatDivider, 
    MatSelectionList, 
    MatListOption,
    MatButton,
    FormsModule
  ],
  templateUrl: './filters-dialog.component.html',
  styleUrl: './filters-dialog.component.scss'
})
export class FiltersDialogComponent {
  portalService = inject(PortalService);
  private dialogRef = inject(MatDialogRef<FiltersDialogComponent>);
  data = inject(MAT_DIALOG_DATA);
  selectedCategories: string[] = this.data.selectedCategories;
  selectedUrgencyLevels: string[] = this.data.selectedUrgencyLevels;

  applyFilters(){
    this.dialogRef.close({
      selectedCategories : this.selectedCategories,
      selectedUrgencyLevels : this.selectedUrgencyLevels,
    })
    
  }
  
}
