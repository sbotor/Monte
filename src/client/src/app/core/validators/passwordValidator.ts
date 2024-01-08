import { AbstractControl, ValidationErrors } from '@angular/forms';

const minLength = 6;

type ErrorKeys = 'passUppercase' | 'passLowercase' | 'passDigit' | 'passNonAlpha' | 'passMinLength';

type Errors = {
  [K in ErrorKeys]: string | null;
};

const errorKeys: (keyof Errors)[] = [
  'passUppercase',
  'passLowercase',
  'passDigit',
  'passNonAlpha',
  'passMinLength',
];

const isUppercase = (c: number) => {
  return c >= 65 && c <= 90;
};

const isLowercase = (c: number) => {
  return c >= 97 && c <= 122;
};

const isDigit = (c: number) => {
  return c >= 48 && c <= 57;
}

export const passwordValidator = (
  control: AbstractControl
): ValidationErrors | null => {
  const value = control.value as string;
  const errors: Errors = {
    passUppercase: 'Uppercase character is required',
    passLowercase: 'Lowercase character is required',
    passDigit: 'Digit is required',
    passNonAlpha: 'Non-alphanumeric character is required',
    passMinLength: `At least ${minLength} characters are required`,
  };

  if (value.length >= minLength) {
    errors.passMinLength = null;
  }

  for (let i = 0; i < value.length; i++) {
    const c = value.charCodeAt(i);
    console.log(c);

    if (isUppercase(c)) {
      errors.passUppercase = null;
    } else if (isLowercase(c)) {
      errors.passLowercase = null;
    } else if (isDigit(c)) {
      errors.passDigit = null;
    } else {
      errors.passNonAlpha = null;
    }
  }

  let hasErrors = false;
  const validationErrors: ValidationErrors = {};

  for (const key of errorKeys) {
    const err = errors[key];
    if (err === null) {
      continue;
    }

    hasErrors = true;
    validationErrors[key] = err;
  }

  return hasErrors ? validationErrors : null;
};

export default passwordValidator;
