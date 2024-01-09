import { Component, EventEmitter, OnDestroy, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { InputFieldComponent } from '@components/input-field/input-field.component';
import { MatInputModule } from '@angular/material/input';
import { InputValidationService } from '@core/input-validation.service';
import { Subject, distinctUntilChanged, takeUntil } from 'rxjs';

export interface NewUserFormValues {
  username: string;
  password: string;
}

@Component({
  selector: 'app-new-user-form',
  standalone: true,
  imports: [
    CommonModule,
    MatInputModule,
    InputFieldComponent,
    MatButtonModule,
    ReactiveFormsModule,
    MatSidenavModule,
  ],
  templateUrl: './new-user-form.component.html',
  styleUrl: './new-user-form.component.scss',
})
export class NewUserFormComponent implements OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  public readonly formGroup = this.fb.nonNullable.group({
    username: ['', [this.validators.required, this.validators.maxLength(50)]],
    password: [
      '',
      [
        this.validators.required,
        this.validators.password,
        this.validators.maxLength(100),
      ],
    ],
    passwordConfirmation: [
      '',
      [this.validators.required, this.validators.maxLength(100)],
    ],
  });

  @Output('submitted') public submitEvent = new EventEmitter<NewUserFormValues>();

  constructor(
    private readonly fb: FormBuilder,
    private readonly validators: InputValidationService
  ) {
    this.formGroup.controls.password.errors;

    this.formGroup.valueChanges
      .pipe(
        takeUntil(this.destroyed$),
        distinctUntilChanged(
          (p, c) =>
            p.password === c.password &&
            p.passwordConfirmation === c.passwordConfirmation
        )
      )
      .subscribe((x) =>
        this.validators.verifyPasswordConfirmation(
          this.formGroup.controls,
          x.password,
          x.passwordConfirmation
        )
      );
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  public onSubmit() {
    if (
      !(
        this.formGroup.valid &&
        this.validators.verifyPasswordConfirmation(this.formGroup.controls)
      )
    ) {
      this.submitEvent.emit(undefined);
    }

    const controls = this.formGroup.controls;
    this.submitEvent.emit({
      username: controls.username.value,
      password: controls.password.value,
    });
  }
}
