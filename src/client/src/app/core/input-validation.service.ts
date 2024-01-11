import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import passwordValidator from './validators/passwordValidator';

export interface PasswordConfirmationForm {
  password: AbstractControl<string>;
  passwordConfirmation: AbstractControl<string>;
}

@Injectable({
  providedIn: 'root',
})
export class InputValidationService {
  private readonly errorKeys = ['required', 'passConfirmation'];

  public required<T = any>(
    control: AbstractControl<T>
  ): ValidationErrors | null {
    const empty = !control.value;
    return empty ? { required: 'This field is required' } : null;
  }

  public password(control: AbstractControl<string>): ValidationErrors | null {
    return passwordValidator(control);
  }

  public maxLength(length: number): ValidatorFn {
    return (control: AbstractControl<string>) =>
      InputValidationService.validateLength(control, undefined, length);
  }

  public username(control: AbstractControl<string>): ValidationErrors | null {
    const res = InputValidationService.validateLength(control, 3, 50);
    return res;
  }

  public validatePasswordConfirmation(
    password: string,
    confirmationControl: AbstractControl<string>
  ) {
    if (password === confirmationControl.value) {
      return true;
    }

    let errors = confirmationControl.errors;
    if (!errors) {
      errors = {};
    }
    errors['passConfirmation'] = 'Passwords do not match';

    confirmationControl.setErrors(errors);

    return false;
  }

  public getFirstError(
    errors: ValidationErrors | null
  ): { key: string; value: string } | null {
    if (!errors) {
      return null;
    }

    for (const key in this.errorKeys) {
      if (errors.hasOwnProperty(key)) {
        return {
          key,
          value: errors[key] as string,
        };
      }
    }

    const firstKey = Object.keys(errors)[0];
    return { key: firstKey, value: errors[firstKey] as string };
  }

  private static validateLength(
    control: AbstractControl<string>,
    min?: number,
    max?: number
  ): ValidationErrors | null {
    const value = control.value;

    if (value === undefined || value === null) {
      return null;
    }

    if (min !== undefined && value.length < min) {
      return { minLength: `Minimum length is ${min}` };
    }

    if (max !== undefined && value.length > max) {
      return { maxLength: `Maximum length is ${max}` };
    }

    return null;
  }
}
