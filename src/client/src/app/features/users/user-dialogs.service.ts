import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
  ConfirmationDialogComponent,
  ConfirmationDialogData,
} from '@components/confirmation-dialog/confirmation-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class UserDialogsService {
  constructor(private readonly dialog: MatDialog) {}

  public openDeleteConfirmationDialog(userName: string) {
    const data: ConfirmationDialogData = {
      title: 'Confirm user deletion',
      content: `Do you really want to delete user <b>${userName}<b>?`,
      confirmButton: { color: 'warn' },
    };

    return this.dialog
      .open<ConfirmationDialogComponent, ConfirmationDialogData, boolean>(
        ConfirmationDialogComponent,
        { data }
      )
      .afterClosed();
  }
}
