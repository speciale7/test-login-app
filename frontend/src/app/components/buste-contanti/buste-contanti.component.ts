import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BusteService } from '../../services/buste.service';
import { Busta, CreateBustaDto } from '../../models/busta.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-buste-contanti',
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
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './buste-contanti.component.html',
  styleUrls: ['./buste-contanti.component.css']
})
export class BusteContantiComponent implements OnInit {
  displayedColumns: string[] = [
    'dataRiferimento',
    'dataChiusura',
    'dataRitiro',
    'sigillo',
    'totale',
    'note',
    'userChiusura',
    'userRitiro',
    'azioni'
  ];
  
  dataSource: Busta[] = [];
  startDate: Date | null = null;
  endDate: Date | null = null;
  isLoading = false;

  constructor(
    private busteService: BusteService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    public authService: AuthService
  ) {
    // Set default date range (current month)
    const now = new Date();
    this.startDate = new Date(now.getFullYear(), now.getMonth(), 1);
    this.endDate = new Date(now.getFullYear(), now.getMonth() + 1, 0);
  }

  ngOnInit(): void {
    this.loadBuste();
  }

  loadBuste(): void {
    this.isLoading = true;
    this.busteService.getBuste(this.startDate || undefined, this.endDate || undefined)
      .subscribe({
        next: (buste) => {
          this.dataSource = buste;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading buste:', error);
          this.snackBar.open('Errore nel caricamento delle buste', 'Chiudi', { duration: 3000 });
          this.isLoading = false;
        }
      });
  }

  onDateRangeChange(): void {
    this.loadBuste();
  }

  openNewBustaDialog(): void {
    const dialogRef = this.dialog.open(BustaDialogComponent, {
      width: '600px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadBuste();
      }
    });
  }

  openEditBustaDialog(busta: Busta): void {
    const dialogRef = this.dialog.open(BustaDialogComponent, {
      width: '600px',
      data: { mode: 'edit', busta }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadBuste();
      }
    });
  }

  duplicateBusta(busta: Busta): void {
    this.busteService.duplicateBusta(busta.id).subscribe({
      next: () => {
        this.snackBar.open('Busta duplicata con successo', 'Chiudi', { duration: 3000 });
        this.loadBuste();
      },
      error: (error) => {
        console.error('Error duplicating busta:', error);
        this.snackBar.open('Errore nella duplicazione', 'Chiudi', { duration: 3000 });
      }
    });
  }

  deleteBusta(busta: Busta): void {
    if (confirm('Sei sicuro di voler eliminare questa busta?')) {
      this.busteService.deleteBusta(busta.id).subscribe({
        next: () => {
          this.snackBar.open('Busta eliminata con successo', 'Chiudi', { duration: 3000 });
          this.loadBuste();
        },
        error: (error) => {
          console.error('Error deleting busta:', error);
          this.snackBar.open('Errore nell\'eliminazione', 'Chiudi', { duration: 3000 });
        }
      });
    }
  }

  formatDate(date: string | null | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('it-IT');
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR'
    }).format(amount);
  }
}

// Dialog Component
@Component({
  selector: 'app-busta-dialog',
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
    <h2 mat-dialog-title>{{ data.mode === 'create' ? 'Nuova Busta' : (authService.canWrite() ? 'Modifica Busta' : 'Visualizza Busta') }}</h2>
    <mat-dialog-content>
      <form [formGroup]="bustaForm">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data Riferimento</mat-label>
          <input matInput [matDatepicker]="pickerRif" formControlName="dataRiferimento" required>
          <mat-datepicker-toggle matSuffix [for]="pickerRif" [disabled]="!authService.canWrite()"></mat-datepicker-toggle>
          <mat-datepicker #pickerRif></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data Chiusura</mat-label>
          <input matInput [matDatepicker]="pickerChiusura" formControlName="dataChiusura">
          <mat-datepicker-toggle matSuffix [for]="pickerChiusura" [disabled]="!authService.canWrite()"></mat-datepicker-toggle>
          <mat-datepicker #pickerChiusura></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data Ritiro</mat-label>
          <input matInput [matDatepicker]="pickerRitiro" formControlName="dataRitiro">
          <mat-datepicker-toggle matSuffix [for]="pickerRitiro" [disabled]="!authService.canWrite()"></mat-datepicker-toggle>
          <mat-datepicker #pickerRitiro></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Sigillo</mat-label>
          <input matInput formControlName="sigillo">
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Totale</mat-label>
          <input matInput type="number" formControlName="totale" required step="0.01">
          <span matPrefix>€&nbsp;</span>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Note</mat-label>
          <textarea matInput formControlName="note" rows="3"></textarea>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>User Chiusura</mat-label>
          <input matInput formControlName="userChiusura">
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>User Ritiro</mat-label>
          <input matInput formControlName="userRitiro">
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">{{ authService.canWrite() ? 'Annulla' : 'Chiudi' }}</button>
      <button mat-raised-button color="primary" (click)="onSave()" 
              [disabled]="!bustaForm.valid || !authService.canWrite()"
              *ngIf="authService.canWrite()">
        {{ data.mode === 'create' ? 'Crea' : 'Salva' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }
    mat-dialog-content {
      min-height: 400px;
      padding-top: 20px;
    }
  `]
})
export class BustaDialogComponent implements OnInit {
  bustaForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<BustaDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'create' | 'edit', busta?: Busta },
    private fb: FormBuilder,
    private busteService: BusteService,
    private snackBar: MatSnackBar,
    public authService: AuthService
  ) {
    this.bustaForm = this.fb.group({
      dataRiferimento: [new Date(), Validators.required],
      dataChiusura: [null],
      dataRitiro: [null],
      sigillo: [''],
      totale: [0, [Validators.required, Validators.min(0)]],
      note: [''],
      userChiusura: [''],
      userRitiro: ['']
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.busta) {
      const busta = this.data.busta;
      this.bustaForm.patchValue({
        dataRiferimento: new Date(busta.dataRiferimento),
        dataChiusura: busta.dataChiusura ? new Date(busta.dataChiusura) : null,
        dataRitiro: busta.dataRitiro ? new Date(busta.dataRitiro) : null,
        sigillo: busta.sigillo || '',
        totale: busta.totale,
        note: busta.note || '',
        userChiusura: busta.userChiusura || '',
        userRitiro: busta.userRitiro || ''
      });
    }

    // Disable all form controls if user is Reader
    if (!this.authService.canWrite()) {
      this.bustaForm.disable();
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.bustaForm.valid) {
      const formValue = this.bustaForm.value;
      const bustaData = {
        dataRiferimento: formValue.dataRiferimento.toISOString(),
        dataChiusura: formValue.dataChiusura ? formValue.dataChiusura.toISOString() : null,
        dataRitiro: formValue.dataRitiro ? formValue.dataRitiro.toISOString() : null,
        sigillo: formValue.sigillo || null,
        totale: formValue.totale,
        note: formValue.note || null,
        userChiusura: formValue.userChiusura || null,
        userRitiro: formValue.userRitiro || null
      };

      if (this.data.mode === 'create') {
        this.busteService.createBusta(bustaData).subscribe({
          next: () => {
            this.snackBar.open('Busta creata con successo', 'Chiudi', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Error creating busta:', error);
            this.snackBar.open('Errore nella creazione', 'Chiudi', { duration: 3000 });
          }
        });
      } else if (this.data.busta) {
        this.busteService.updateBusta(this.data.busta.id, bustaData).subscribe({
          next: () => {
            this.snackBar.open('Busta aggiornata con successo', 'Chiudi', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Error updating busta:', error);
            this.snackBar.open('Errore nell\'aggiornamento', 'Chiudi', { duration: 3000 });
          }
        });
      }
    }
  }
}

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject } from '@angular/core';

