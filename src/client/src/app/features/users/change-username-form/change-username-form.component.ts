import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, NgForm, ReactiveFormsModule } from '@angular/forms';
import { InputValidationService } from '@core/input-validation.service';
import { InputFieldComponent } from '@components/input-field/input-field.component';
import { SpinnerComponent } from '@components/spinner';
import { MatButtonModule } from '@angular/material/button';

export interface ChangeUsernameFormValues {
  username: string;
  reset: () => void;
}

@Component({
  selector: 'app-change-username-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputFieldComponent,
    SpinnerComponent,
    MatButtonModule,
  ],
  templateUrl: './change-username-form.component.html',
  styleUrl: './change-username-form.component.scss',
})
export class ChangeUsernameFormComponent implements OnChanges {
  public readonly formGroup = this.fb.nonNullable.group({
    username: ['', [this.validators.required, this.validators.username]],
  });

  @Input() public isLoading = false;

  @Output('submitted') public submitEvent = new EventEmitter<ChangeUsernameFormValues>();

  @ViewChild('form') form: NgForm = null!;

  constructor(
    private readonly fb: FormBuilder,
    private readonly validators: InputValidationService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    const isLoading = changes['isLoading'];

    const controls = this.formGroup.controls;

    if (isLoading.currentValue) {
      controls.username.disable();
    } else {
      controls.username.enable();
    }
  }

  public onSubmit() {
    if (this.formGroup.invalid) {
      this.submitEvent.emit(undefined);
    }

    this.submitEvent.emit({
      username: this.formGroup.controls.username.value,
      reset: () => this.form.reset()
    });
  }
}
