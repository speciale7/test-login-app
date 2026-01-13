import { Component, OnInit, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { UsersService } from '../../services/users.service';
import { AuthService } from '../../services/auth.service';
import { UserDto, CreateUserDto, UpdateUserDto } from '../../models/user.model';
import { UserRole } from '../../models/auth.model';

@Component({
  selector: 'app-user-management',
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
    MatSelectModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatChipsModule
  ],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  displayedColumns: string[] = ['id', 'username', 'email', 'role', 'createdAt', 'actions'];
  dataSource: UserDto[] = [];
  isLoading = false;
  currentUser: any;
  UserRole = UserRole;

  constructor(
    private usersService: UsersService,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.currentUser = this.authService.currentUser();
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.usersService.getUsers().subscribe({
      next: (users) => {
        this.dataSource = users;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.snackBar.open('Errore nel caricamento degli utenti', 'Chiudi', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  getRoleName(role: UserRole): string {
    switch (role) {
      case UserRole.Admin: return 'Admin';
      case UserRole.Writer: return 'Writer';
      case UserRole.Reader: return 'Reader';
      default: return 'Unknown';
    }
  }

  getRoleColor(role: UserRole): string {
    switch (role) {
      case UserRole.Admin: return 'primary';
      case UserRole.Writer: return 'accent';
      case UserRole.Reader: return 'warn';
      default: return '';
    }
  }

  openCreateUserDialog(): void {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '500px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  openEditUserDialog(user: UserDto): void {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '500px',
      data: { mode: 'edit', user }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  deleteUser(user: UserDto): void {
    if (user.id === this.currentUser?.id) {
      this.snackBar.open('Non puoi eliminare il tuo account', 'Chiudi', { duration: 3000 });
      return;
    }

    if (confirm(`Sei sicuro di voler eliminare l'utente ${user.username}?`)) {
      this.usersService.deleteUser(user.id).subscribe({
        next: () => {
          this.snackBar.open('Utente eliminato con successo', 'Chiudi', { duration: 3000 });
          this.loadUsers();
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          this.snackBar.open('Errore nell\'eliminazione', 'Chiudi', { duration: 3000 });
        }
      });
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('it-IT', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}

// Dialog Component
@Component({
  selector: 'app-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>{{ data.mode === 'create' ? 'Nuovo Utente' : 'Modifica Utente' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="userForm">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Username</mat-label>
          <input matInput formControlName="username" required>
          <mat-error *ngIf="userForm.get('username')?.hasError('required')">
            Username è richiesto
          </mat-error>
          <mat-error *ngIf="userForm.get('username')?.hasError('minlength')">
            Minimo 3 caratteri
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" required>
          <mat-error *ngIf="userForm.get('email')?.hasError('required')">
            Email è richiesta
          </mat-error>
          <mat-error *ngIf="userForm.get('email')?.hasError('email')">
            Email non valida
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width" *ngIf="data.mode === 'create'">
          <mat-label>Password</mat-label>
          <input matInput type="password" formControlName="password" required>
          <mat-error *ngIf="userForm.get('password')?.hasError('required')">
            Password è richiesta
          </mat-error>
          <mat-error *ngIf="userForm.get('password')?.hasError('minlength')">
            Minimo 6 caratteri
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width" *ngIf="data.mode === 'edit'">
          <mat-label>Nuova Password (opzionale)</mat-label>
          <input matInput type="password" formControlName="password">
          <mat-hint>Lascia vuoto per non modificare</mat-hint>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Ruolo</mat-label>
          <mat-select formControlName="role" required>
            <mat-option [value]="UserRole.Reader">Reader (Solo Lettura)</mat-option>
            <mat-option [value]="UserRole.Writer">Writer (Modifica)</mat-option>
            <mat-option [value]="UserRole.Admin">Admin (Completo)</mat-option>
          </mat-select>
          <mat-error *ngIf="userForm.get('role')?.hasError('required')">
            Ruolo è richiesto
          </mat-error>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Annulla</button>
      <button mat-raised-button color="primary" (click)="onSave()" [disabled]="!userForm.valid">
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
      min-height: 300px;
      padding-top: 20px;
    }
  `]
})
export class UserDialogComponent implements OnInit {
  userForm: FormGroup;
  UserRole = UserRole;

  constructor(
    public dialogRef: MatDialogRef<UserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'create' | 'edit', user?: UserDto },
    private fb: FormBuilder,
    private usersService: UsersService,
    private snackBar: MatSnackBar
  ) {
    this.userForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: [data.mode === 'create' ? '' : null, data.mode === 'create' ? [Validators.required, Validators.minLength(6)] : []],
      role: [UserRole.Reader, Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.user) {
      const user = this.data.user;
      this.userForm.patchValue({
        username: user.username,
        email: user.email,
        role: user.role
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.userForm.valid) {
      const formValue = this.userForm.value;
      
      if (this.data.mode === 'create') {
        const createData: CreateUserDto = {
          username: formValue.username,
          email: formValue.email,
          password: formValue.password,
          role: formValue.role
        };

        this.usersService.createUser(createData).subscribe({
          next: () => {
            this.snackBar.open('Utente creato con successo', 'Chiudi', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Error creating user:', error);
            const message = error.error?.message || 'Errore nella creazione';
            this.snackBar.open(message, 'Chiudi', { duration: 3000 });
          }
        });
      } else if (this.data.user) {
        const updateData: UpdateUserDto = {
          username: formValue.username,
          email: formValue.email,
          role: formValue.role
        };
        
        if (formValue.password) {
          updateData.password = formValue.password;
        }

        this.usersService.updateUser(this.data.user.id, updateData).subscribe({
          next: () => {
            this.snackBar.open('Utente aggiornato con successo', 'Chiudi', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Error updating user:', error);
            const message = error.error?.message || 'Errore nell\'aggiornamento';
            this.snackBar.open(message, 'Chiudi', { duration: 3000 });
          }
        });
      }
    }
  }
}
