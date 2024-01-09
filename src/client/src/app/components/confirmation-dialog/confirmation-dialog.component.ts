import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

export interface ConfirmationDialogButton {
  label?: string;
  color?: string;
}

export interface ConfirmationDialogData {
  title?: string;
  content?: string;
  confirmButton?: ConfirmationDialogButton;
  cancelButton?: ConfirmationDialogButton;
}

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatDialogModule],
  templateUrl: './confirmation-dialog.component.html',
  styleUrl: './confirmation-dialog.component.scss',
})
export class ConfirmationDialogComponent {
  public readonly content: SafeHtml | null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public readonly data: ConfirmationDialogData,
    sanitizer: DomSanitizer
  ) {
    this.content = data.content
      ? sanitizer.bypassSecurityTrustHtml(data.content)
      : null;
  }
}
