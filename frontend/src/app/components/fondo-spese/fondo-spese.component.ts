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
import { FondoSpeseService } from '../../services/fondo-spese.service';
import { FondoSpese, CreateFondoSpeseDto, UpdateFondoSpeseDto } from '../../models/cash-management.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-fondo-spese',
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
  templateUrl: './fondo-spese.component.html',
  styleUrls: ['./fondo-spese.component.css']
})
export class FondoSpeseComponent implements OnInit {
  displayedColumns: string[] = ['countingDate', 'expenseType', 'countingAmount', 'invoiceNumber', 'reasonExpenses', 'actions'];
  dataSource: FondoSpese[] = [];
  isLoading = false;

  // Summary data
  fondoIniziale = 500;
  rimanente = 350;
  numFatture = 0;
  pagamentiTotali = 0;
  lastUpdate?: Date;

  constructor(
    private fondoSpeseService: FondoSpeseService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.fondoSpeseService.getAll().subscribe({
      next: (data) => {
        this.dataSource = data;
        this.calculateSummary();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading data:', error);
        this.snackBar.open('Errore nel caricamento dei dati', 'Chiudi', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  calculateSummary(): void {
    this.numFatture = this.dataSource.length;
    this.pagamentiTotali = this.dataSource.reduce((sum, item) => sum + (item.countingAmount || 0), 0);
    this.rimanente = this.fondoIniziale - this.pagamentiTotali;
    if (this.dataSource.length > 0 && this.dataSource[0].countingAt) {
      this.lastUpdate = new Date(this.dataSource[0].countingAt);
    }
  }

  deleteItem(id: number): void {
    if (confirm('Sei sicuro di voler eliminare questo elemento?')) {
      this.fondoSpeseService.delete(id).subscribe({
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

  openNewDialog(): void {
    const dialogRef = this.dialog.open(FondoSpeseDialogComponent, {
      width: '750px',
      data: { mode: 'create', fondoIniziale: this.fondoIniziale }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }

  openEditDialog(item: FondoSpese): void {
    const dialogRef = this.dialog.open(FondoSpeseDialogComponent, {
      width: '750px',
      data: { mode: 'edit', item, fondoIniziale: this.fondoIniziale }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }
}

@Component({
  selector: 'app-fondo-spese-dialog',
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
      {{ data.mode === 'create' ? 'Nuova Spesa' : (authService.canWrite() ? 'Modifica Spesa' : 'Visualizza Spesa') }}
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
          <mat-label>Tipo Spesa</mat-label>
          <input matInput formControlName="expenseType">
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

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data Fattura</mat-label>
          <input matInput [matDatepicker]="pickerInvoice" formControlName="invoiceDate">
          <mat-datepicker-toggle matSuffix [for]="pickerInvoice" [disabled]="!authService.canWrite()"></mat-datepicker-toggle>
          <mat-datepicker #pickerInvoice></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>N. Fattura</mat-label>
          <input matInput formControlName="invoiceNumber">
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Causale</mat-label>
          <textarea matInput formControlName="reasonExpenses" rows="3"></textarea>
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
    mat-dialog-content { min-height: 520px; padding-top: 20px; }
  `]
})
export class FondoSpeseDialogComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<FondoSpeseDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'create' | 'edit'; item?: FondoSpese; fondoIniziale?: number },
    private fb: FormBuilder,
    private service: FondoSpeseService,
    private snackBar: MatSnackBar,
    public authService: AuthService
  ) {
    this.form = this.fb.group({
      idStore: [371, [Validators.required, Validators.min(1)]],
      countingDate: [new Date(), [Validators.required]],
      expenseType: [''],
      countingAmount: [null, [Validators.required]],
      countingCoins: [null],
      invoiceDate: [null],
      invoiceNumber: [''],
      reasonExpenses: ['']
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.item) {
      this.form.patchValue({
        idStore: this.data.item.idStore,
        countingDate: this.data.item.countingDate ? new Date(this.data.item.countingDate) : null,
        expenseType: this.data.item.expenseType ?? '',
        countingAmount: this.data.item.countingAmount ?? null,
        countingCoins: this.data.item.countingCoins ?? null,
        invoiceDate: this.data.item.invoiceDate ? new Date(this.data.item.invoiceDate) : null,
        invoiceNumber: this.data.item.invoiceNumber ?? '',
        reasonExpenses: this.data.item.reasonExpenses ?? ''
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
      const payload: CreateFondoSpeseDto = this.form.value;
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

    const updatePayload: UpdateFondoSpeseDto = {
      countingDate: this.form.value.countingDate,
      expenseType: this.form.value.expenseType,
      countingAmount: this.form.value.countingAmount,
      countingCoins: this.form.value.countingCoins,
      invoiceDate: this.form.value.invoiceDate,
      invoiceNumber: this.form.value.invoiceNumber,
      reasonExpenses: this.form.value.reasonExpenses
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
