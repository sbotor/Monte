import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { InputFieldComponent } from '@components/input-field/input-field.component';
import { MatButtonModule } from '@angular/material/button';
import { InputValidationService } from '@core/input-validation.service';

@Component({
  selector: 'app-create-user-form',
  standalone: true,
  imports: [
    CommonModule,
    MatInputModule,
    InputFieldComponent,
    MatButtonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './create-user-form.component.html',
  styleUrl: './create-user-form.component.scss',
})
export class CreateUserFormComponent {
  public readonly formGroup = this.fb.nonNullable.group({
    username: ['', [this.validators.required, this.validators.maxLength(50)]],
    password: ['', [this.validators.required, this.validators.password, this.validators.maxLength(100)]],
    passwordConfirmation: ['', this.validators.required],
  });

  constructor(private readonly fb: FormBuilder, private readonly validators: InputValidationService) {
    this.formGroup.controls.password.errors;
  }

  public onSubmit() {
    console.log(this.formGroup.controls);
  }
}
