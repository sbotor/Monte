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
    return (control: AbstractControl<string>): ValidationErrors | null => {
      const isValid = !control.value || control.value.length <= length;

      return isValid ? null : { maxLength: `Maximum length is ${length}` };
    };
  }

  public verifyPasswordConfirmation(
    form: PasswordConfirmationForm,
    password?: string,
    passwordConfirmation?: string
  ) {
    const passValue = password || form.password.value;
    const confValue = passwordConfirmation || form.passwordConfirmation.value;

    if (passValue === confValue) {
      return true;
    }

    let errors = form.passwordConfirmation.errors;
    if (!errors) {
      errors = {};
    }
    errors['passConfirmation'] = 'Passwords do not match';

    form.passwordConfirmation.setErrors(errors);

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
}
