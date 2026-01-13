import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SovvMonetariaService } from '../../services/sovv-monetaria.service';
import { SovvMonetaria, CreateSovvMonetariaDto, UpdateSovvMonetariaDto } from '../../models/cash-management.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sovv-monetaria',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  templateUrl: './sovv-monetaria.component.html',
  styleUrls: ['./sovv-monetaria.component.css']
})
export class SovvMonetariaComponent implements OnInit {
  isLoading = false;
  fondo = 0;
  rimanente = 0;
  lastUpdate?: Date;

  displayedColumns: string[] = ['countingDate', 'countingAmount', 'countingNote', 'countingCoins', 'actions'];
  dataSource: SovvMonetaria[] = [];

  constructor(
    private sovvMonetariaService: SovvMonetariaService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.sovvMonetariaService.getAll().subscribe({
      next: (data) => {
        this.dataSource = data;
        if (data.length > 0) {
          const latest = data[0];
          this.fondo = latest.countingAmount || 0;
          this.rimanente = this.fondo * 0.77; // Example calculation
          if (latest.countingAt) {
            this.lastUpdate = new Date(latest.countingAt);
          }
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading data:', error);
        this.snackBar.open('Errore nel caricamento dei dati', 'Chiudi', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  openNewDialog(): void {
    const dialogRef = this.dialog.open(SovvMonetariaDialogComponent, {
      width: '700px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }

  openEditDialog(item: SovvMonetaria): void {
    const dialogRef = this.dialog.open(SovvMonetariaDialogComponent, {
      width: '700px',
      data: { mode: 'edit', item }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }

  deleteItem(id: number): void {
    if (confirm('Sei sicuro di voler eliminare questo elemento?')) {
      this.sovvMonetariaService.delete(id).subscribe({
        next: () => {
          this.snackBar.open('Elemento eliminato con successo', 'Chiudi', { duration: 3000 });
          this.loadData();
        },
        error: (error) => {
          console.error('Error deleting item:', error);
          this.snackBar.open('Errore nell\'eliminazione', 'Chiudi', { duration: 3000 });
        }
      });
    }
  }
}

@Component({
  selector: 'app-sovv-monetaria-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  template: `
    <h2 mat-dialog-title>
      {{ data.mode === 'create' ? 'Nuova Sovv. Monetaria' : (authService.canWrite() ? 'Modifica Sovv. Monetaria' : 'Visualizza Sovv. Monetaria') }}
    </h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Id Store</mat-label>
          <input matInput type="number" formControlName="idStore" required>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data</mat-label>
          <input matInput [matDatepicker]="pickerDate" formControlName="countingDate" required>
          <mat-datepicker-toggle matSuffix [for]="pickerDate" [disabled]="!authService.canWrite()"></mat-datepicker-toggle>
          <mat-datepicker #pickerDate></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Importo</mat-label>
          <input matInput type="number" formControlName="countingAmount" step="0.01" required>
          <span matPrefix>€&nbsp;</span>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Note</mat-label>
          <textarea matInput formControlName="countingNote" rows="3"></textarea>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Monete</mat-label>
          <input matInput type="number" formControlName="countingCoins" step="0.01">
          <span matPrefix>€&nbsp;</span>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">{{ authService.canWrite() ? 'Annulla' : 'Chiudi' }}</button>
      <button mat-raised-button color="primary" (click)="onSave()"
              [disabled]="!form.valid || !authService.canWrite()"
              *ngIf="authService.canWrite()">
        {{ data.mode === 'create' ? 'Crea' : 'Salva' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .full-width { width: 100%; margin-bottom: 16px; }
    mat-dialog-content { min-height: 420px; padding-top: 20px; }
  `]
})
export class SovvMonetariaDialogComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<SovvMonetariaDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'create' | 'edit'; item?: SovvMonetaria },
    private fb: FormBuilder,
    private service: SovvMonetariaService,
    private snackBar: MatSnackBar,
    public authService: AuthService
  ) {
    this.form = this.fb.group({
      idStore: [371, [Validators.required, Validators.min(1)]],
      countingDate: [new Date(), [Validators.required]],
      countingAmount: [null, [Validators.required]],
      countingNote: [''],
      countingCoins: [null]
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.item) {
      this.form.patchValue({
        idStore: this.data.item.idStore,
        countingDate: this.data.item.countingDate ? new Date(this.data.item.countingDate) : null,
        countingAmount: this.data.item.countingAmount ?? null,
        countingNote: this.data.item.countingNote ?? '',
        countingCoins: this.data.item.countingCoins ?? null
      });
    }

    if (!this.authService.canWrite()) {
      this.form.disable();
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onSave(): void {
    if (!this.authService.canWrite() || !this.form.valid) {
      return;
    }

    if (this.data.mode === 'create') {
      const payload: CreateSovvMonetariaDto = this.form.value;
      this.service.create(payload).subscribe({
        next: () => {
          this.snackBar.open('Record creato con successo', 'Chiudi', { duration: 3000 });
          this.dialogRef.close(true);
        },
        error: (error) => {
          console.error('Error creating item:', error);
          this.snackBar.open('Errore nella creazione', 'Chiudi', { duration: 3000 });
        }
      });
      return;
    }

    if (!this.data.item) {
      return;
    }

    const updatePayload: UpdateSovvMonetariaDto = {
      countingDate: this.form.value.countingDate,
      countingAmount: this.form.value.countingAmount,
      countingNote: this.form.value.countingNote,
      countingCoins: this.form.value.countingCoins
    };

    this.service.update(this.data.item.id, updatePayload).subscribe({
      next: () => {
        this.snackBar.open('Record aggiornato con successo', 'Chiudi', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (error) => {
        console.error('Error updating item:', error);
        this.snackBar.open('Errore nell\'aggiornamento', 'Chiudi', { duration: 3000 });
      }
    });
  }
}
