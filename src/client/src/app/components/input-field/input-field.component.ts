import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormGroup,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { InputValidationService } from '@core/input-validation.service';

@Component({
  selector: 'app-input-field',
  standalone: true,
  imports: [CommonModule, MatInputModule, ReactiveFormsModule],
  templateUrl: './input-field.component.html',
  styleUrl: './input-field.component.scss',
})
export class InputFieldComponent implements OnInit {
  @Input() public controlName: string = null!;
  @Input() public group: FormGroup = null!;
  @Input() public label?: string;
  @Input() public type = 'text';

  private control?: AbstractControl;

  constructor(private readonly validator: InputValidationService) {}

  ngOnInit(): void {
    if (!this.group || !this.controlName) {
      return;
    }

    this.control = this.group.controls[this.controlName];
  }

  public getErrorMsg() {
    if (!this.control) {
      return null;
    }

    return this.validator.getFirstError(this.control.errors)?.value;
  }
}
