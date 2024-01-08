import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import passwordValidator from './validators/passwordValidator';

@Injectable({
  providedIn: 'root',
})
export class InputValidationService {
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

  public getFirstError(
    errors: ValidationErrors | null
  ): { key: string; value: string } | null {
    if (!errors) {
      return null;
    }

    if ('required' in errors) {
      return {
        key: 'required',
        value: errors['required'] as string,
      };
    }

    const firstKey = Object.keys(errors)[0];
    return { key: firstKey, value: errors[firstKey] as string };
  }
}
