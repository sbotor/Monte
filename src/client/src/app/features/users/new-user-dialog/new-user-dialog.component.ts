import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { NewUserFormComponent, NewUserFormValues } from '../new-user-form/new-user-form.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-new-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    NewUserFormComponent,
    MatButtonModule,
  ],
  templateUrl: './new-user-dialog.component.html',
  styleUrl: './new-user-dialog.component.scss',
})
export class NewUserDialogComponent {
  constructor(private readonly dialogRef: MatDialogRef<NewUserDialogComponent>) {}

  public onSubmit(value?: NewUserFormValues) {
    if (!value) {
      return;
    }

    this.dialogRef.close(value);
  }
}
