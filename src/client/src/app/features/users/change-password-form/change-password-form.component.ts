import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  NgForm,
  ReactiveFormsModule,
} from '@angular/forms';
import { InputValidationService } from '@core/input-validation.service';
import { Subject, distinctUntilChanged, takeUntil } from 'rxjs';
import { InputFieldComponent } from '@components/input-field/input-field.component';
import { SpinnerComponent } from '@components/spinner';
import { MatButtonModule } from '@angular/material/button';

export interface ChangePasswordFormValues {
  currentPassword: string;
  newPassword: string;
  reset: () => void;
}

@Component({
  selector: 'app-change-password-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputFieldComponent,
    SpinnerComponent,
    MatButtonModule,
  ],
  templateUrl: './change-password-form.component.html',
  styleUrl: './change-password-form.component.scss',
})
export class ChangePasswordFormComponent implements OnDestroy, OnChanges {
  private readonly destroyed$ = new Subject<void>();

  @Input() public isLoading = false;

  @Output('submitted') public submitEvent =
    new EventEmitter<ChangePasswordFormValues>();

  @ViewChild('form') form: NgForm = null!;

  public readonly formGroup = this.fb.nonNullable.group({
    currentPassword: [
      '',
      [this.validators.required, this.validators.maxLength(100)],
    ],
    newPassword: [
      '',
      [
        this.validators.required,
        this.validators.maxLength(100),
        this.validators.password,
      ],
    ],
    passwordConfirmation: [
      '',
      [this.validators.required, this.validators.maxLength(100)],
    ],
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly validators: InputValidationService
  ) {
    this.formGroup.valueChanges
      .pipe(
        takeUntil(this.destroyed$),
        distinctUntilChanged(
          (p, c) =>
            p.newPassword === c.newPassword &&
            p.passwordConfirmation === c.passwordConfirmation
        )
      )
      .subscribe((_) => {
        const controls = this.formGroup.controls;
        this.validators.validatePasswordConfirmation(
          controls.newPassword.value,
          controls.passwordConfirmation
        );
      });
  }

  ngOnChanges(changes: SimpleChanges): void {
    const isLoading = changes['isLoading'];
    const controls = this.formGroup.controls;

    const action = isLoading.currentValue
      ? (x: AbstractControl) => x.disable()
      : (x: AbstractControl) => x.enable();

    action(controls.currentPassword);
    action(controls.newPassword);
    action(controls.passwordConfirmation);
  }

  public onSubmit() {
    const controls = this.formGroup.controls;
    if (
      !(
        this.formGroup.valid &&
        this.validators.validatePasswordConfirmation(
          controls.newPassword.value,
          controls.passwordConfirmation
        )
      )
    ) {
      this.submitEvent.emit(undefined);
    }

    this.submitEvent.emit({
      currentPassword: controls.currentPassword.value,
      newPassword: controls.newPassword.value,
      reset: () => this.form.reset(),
    });
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
