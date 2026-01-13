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
import { FondoCassaService } from '../../services/fondo-cassa.service';
import { FondoCassa, CreateFondoCassaDto, UpdateFondoCassaDto } from '../../models/cash-management.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-fondo-cassa',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  templateUrl: './fondo-cassa.component.html',
  styleUrls: ['./fondo-cassa.component.css']
})
export class FondoCassaComponent implements OnInit {
  isLoading = false;
  fondiCassa: { code: string; amount: number }[] = [];
  lastUpdate?: Date;

  displayedColumns: string[] = ['countingDate', 'cashCode', 'countingAmount', 'countingCoins', 'actions'];
  dataSource: FondoCassa[] = [];

  constructor(
    private fondoCassaService: FondoCassaService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.fondoCassaService.getAll().subscribe({
      next: (data) => {
        this.dataSource = data;

        // Group by cashCode and get latest for each
        const grouped = data.reduce((acc, item) => {
          const code = item.cashCode || '1';
          if (!acc[code] || (item.countingDate && acc[code].countingDate && item.countingDate > acc[code].countingDate)) {
            acc[code] = item;
          }
          return acc;
        }, {} as { [key: string]: FondoCassa });

        this.fondiCassa = Object.values(grouped).map(item => ({
          code: item.cashCode || '1',
          amount: item.countingAmount || 0
        }));

        if (data.length > 0 && data[0].countingAt) {
          this.lastUpdate = new Date(data[0].countingAt);
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
    const dialogRef = this.dialog.open(FondoCassaDialogComponent, {
      width: '700px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }

  openEditDialog(item: FondoCassa): void {
    const dialogRef = this.dialog.open(FondoCassaDialogComponent, {
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
      this.fondoCassaService.delete(id).subscribe({
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
  selector: 'app-fondo-cassa-dialog',
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
      {{ data.mode === 'create' ? 'Nuovo Fondo Cassa' : (authService.canWrite() ? 'Modifica Fondo Cassa' : 'Visualizza Fondo Cassa') }}
    </h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Id Store</mat-label>
          <input matInput type="number" formControlName="idStore" required>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Cassa (Cash Code)</mat-label>
          <input matInput formControlName="cashCode" required>
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
    mat-dialog-content { min-height: 360px; padding-top: 20px; }
  `]
})
export class FondoCassaDialogComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<FondoCassaDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'create' | 'edit'; item?: FondoCassa },
    private fb: FormBuilder,
    private service: FondoCassaService,
    private snackBar: MatSnackBar,
    public authService: AuthService
  ) {
    this.form = this.fb.group({
      idStore: [371, [Validators.required, Validators.min(1)]],
      cashCode: ['1', [Validators.required]],
      countingDate: [new Date(), [Validators.required]],
      countingAmount: [null, [Validators.required]],
      countingCoins: [null]
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.item) {
      this.form.patchValue({
        idStore: this.data.item.idStore,
        cashCode: this.data.item.cashCode ?? '1',
        countingDate: this.data.item.countingDate ? new Date(this.data.item.countingDate) : null,
        countingAmount: this.data.item.countingAmount ?? null,
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
      const payload: CreateFondoCassaDto = this.form.value;
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

    const updatePayload: UpdateFondoCassaDto = {
      cashCode: this.form.value.cashCode,
      countingDate: this.form.value.countingDate,
      countingAmount: this.form.value.countingAmount,
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
