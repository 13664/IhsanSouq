import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatInput } from '@angular/material/input';
import { MatCard } from '@angular/material/card';
import { MatSelect } from '@angular/material/select';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PortalService } from '../../../core/services/portal.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatOptionModule } from '@angular/material/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-create-charity-case',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCard,
    MatInput,
    MatButton,
    MatSelect,
    MatFormFieldModule,
    MatOptionModule,
  ],
  templateUrl: './charity-case-create.component.html',
  styleUrls: ['./charity-case-create.component.scss'],
})
export class CreateCharityCaseComponent {
  private fb = inject(FormBuilder);
  public portalService = inject(PortalService);
  private router = inject(Router);
  private route = inject(ActivatedRoute); // <-- Inject route params

  editMode = false;
  charityCaseId: number | null = null;

  form: FormGroup = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    amountRequested: [0, [Validators.required, Validators.min(1)]],
    category: ['', Validators.required],
    urgencyLevel: ['', Validators.required],
    endDate: [null], // Optional
    imageUrl: ['', Validators.required],
    beneficiaryName: ['', Validators.required],
    isActive: [true],
  });

  ngOnInit(): void {
    this.InitilizePortal();

    // Check if in edit mode
    this.charityCaseId = this.route.snapshot.params['id'];
    if (this.charityCaseId) {
      this.editMode = true;
      this.loadCharityCase(this.charityCaseId);
    }
  }

  InitilizePortal() {
    this.portalService.getCategories();
    this.portalService.getUrgencyLevels();
  }

  loadCharityCase(id: number) {
    this.portalService.getCharityCase(id).subscribe({
      next: (charityCase) => {
        this.form.patchValue(charityCase); // Pre-fill form
      },
      error: (err) => console.error('Error loading charity case:', err),
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    const charityCase = {
      ...this.form.value,
      id: this.charityCaseId ?? 0, // Include ID for update
      requestDate: new Date(), // Optionally keep this static or backend can manage
      amountCollected: 0, // Optionally keep this static or backend can manage
    };

    if (this.editMode) {
      // Edit mode - PUT
      this.portalService
        .updateCharityCase(this.charityCaseId!, charityCase)
        .subscribe({
          next: () => this.router.navigate(['/portal']),
        });
    } else {
      // Create mode - POST
      this.portalService.createCharityCase(charityCase).subscribe({
        next: () => this.router.navigate(['/portal']),
        error: (err) => console.error(err),
      });
    }
  }
}
