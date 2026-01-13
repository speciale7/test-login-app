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
import { CassaInteligenteService } from '../../services/cassa-intelligente.service';
import { CassaIntelligente, CreateCassaInteligenteDto, UpdateCassaInteligenteDto } from '../../models/cash-management.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-cassa-intelligente',
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
  templateUrl: './cassa-intelligente.component.html',
  styleUrls: ['./cassa-intelligente.component.css']
})
export class CassaInteligenteComponent implements OnInit {
  displayedColumns: string[] = ['countingDate', 'securityEnvelopeCode', 'countingAmount', 'countingDifference', 'withdrawalDate', 'actions'];
  dataSource: CassaIntelligente[] = [];
  isLoading = false;

  // Summary data
  totalAmount = 0;
  lastUpdate?: Date;

  constructor(
    private cassaInteligenteService: CassaInteligenteService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.cassaInteligenteService.getAll().subscribe({
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
    this.totalAmount = this.dataSource.reduce((sum, item) => sum + (item.countingAmount || 0), 0);
    if (this.dataSource.length > 0 && this.dataSource[0].countingAt) {
      this.lastUpdate = new Date(this.dataSource[0].countingAt);
    }
  }

  deleteItem(id: number): void {
    if (confirm('Sei sicuro di voler eliminare questo elemento?')) {
      this.cassaInteligenteService.delete(id).subscribe({
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
    const dialogRef = this.dialog.open(CassaIntelligenteDialogComponent, {
      width: '700px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }

  openEditDialog(item: CassaIntelligente): void {
    const dialogRef = this.dialog.open(CassaIntelligenteDialogComponent, {
      width: '700px',
      data: { mode: 'edit', item }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }
}

@Component({
  selector: 'app-cassa-intelligente-dialog',
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
      {{ data.mode === 'create' ? 'Nuova Cassa Intelligente' : (authService.canWrite() ? 'Modifica Cassa Intelligente' : 'Visualizza Cassa Intelligente') }}
    </h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Id Store</mat-label>
          <input matInput type="number" formControlName="idStore" required>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Store Alias</mat-label>
          <input matInput formControlName="storeAlias">
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data Conta</mat-label>
          <input matInput [matDatepicker]="pickerCounting" formControlName="countingDate" required>
          <mat-datepicker-toggle matSuffix [for]="pickerCounting" [disabled]="!authService.canWrite()"></mat-datepicker-toggle>
          <mat-datepicker #pickerCounting></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Codice Busta</mat-label>
          <input matInput formControlName="securityEnvelopeCode">
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Importo</mat-label>
          <input matInput type="number" formControlName="countingAmount" step="0.01">
          <span matPrefix>€&nbsp;</span>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Differenza</mat-label>
          <input matInput type="number" formControlName="countingDifference" step="0.01">
          <span matPrefix>€&nbsp;</span>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Monete</mat-label>
          <input matInput type="number" formControlName="countingCoins" step="0.01">
          <span matPrefix>€&nbsp;</span>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data Prelievo</mat-label>
          <input matInput [matDatepicker]="pickerWithdrawal" formControlName="withdrawalDate">
          <mat-datepicker-toggle matSuffix [for]="pickerWithdrawal" [disabled]="!authService.canWrite()"></mat-datepicker-toggle>
          <mat-datepicker #pickerWithdrawal></mat-datepicker>
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
export class CassaIntelligenteDialogComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<CassaIntelligenteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'create' | 'edit'; item?: CassaIntelligente },
    private fb: FormBuilder,
    private service: CassaInteligenteService,
    private snackBar: MatSnackBar,
    public authService: AuthService
  ) {
    this.form = this.fb.group({
      idStore: [371, [Validators.required, Validators.min(1)]],
      storeAlias: [''],
      countingDate: [new Date(), [Validators.required]],
      securityEnvelopeCode: [''],
      countingAmount: [null],
      countingDifference: [null],
      countingCoins: [null],
      withdrawalDate: [null]
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.item) {
      this.form.patchValue({
        idStore: this.data.item.idStore,
        storeAlias: this.data.item.storeAlias ?? '',
        countingDate: this.data.item.countingDate ? new Date(this.data.item.countingDate) : null,
        securityEnvelopeCode: this.data.item.securityEnvelopeCode ?? '',
        countingAmount: this.data.item.countingAmount ?? null,
        countingDifference: this.data.item.countingDifference ?? null,
        countingCoins: this.data.item.countingCoins ?? null,
        withdrawalDate: this.data.item.withdrawalDate ? new Date(this.data.item.withdrawalDate) : null
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
      const payload: CreateCassaInteligenteDto = this.form.value;
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

    const updatePayload: UpdateCassaInteligenteDto = {
      storeAlias: this.form.value.storeAlias,
      countingDate: this.form.value.countingDate,
      securityEnvelopeCode: this.form.value.securityEnvelopeCode,
      countingAmount: this.form.value.countingAmount,
      countingDifference: this.form.value.countingDifference,
      countingCoins: this.form.value.countingCoins,
      withdrawalDate: this.form.value.withdrawalDate
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
