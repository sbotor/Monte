import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import passwordValidator from './passwordValidator';

@Component({
  selector: 'app-create-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatInputModule],
  templateUrl: './create-user-form.component.html',
  styleUrl: './create-user-form.component.scss',
})
export class CreateUserFormComponent {
  public readonly formGroup = this.fb.group({
    username: ['', Validators.required],
    password: ['', [Validators.required, passwordValidator]],
    passwordConfirmation: ['', Validators.required],
  });

  constructor(private readonly fb: FormBuilder) {}
}
