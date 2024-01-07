import { AbstractControl, ValidationErrors } from '@angular/forms';

const minLength = 6;

type ErrorKeys = 'uppercase' | 'lowercase' | 'digit' | 'nonAlpha' | 'length';

type Errors = {
  [K in ErrorKeys]: string | null;
};

const errorKeys: (keyof Errors)[] = [
  'uppercase',
  'lowercase',
  'digit',
  'nonAlpha',
  'length',
];

const isUppercase = (c: string) => {
  return c === c.toUpperCase();
};

const isLowercase = (c: string) => {
  return c === c.toLowerCase();
};

const isDigit = (c: string) => {
  return false;
}

export const passwordValidator = (
  control: AbstractControl
): ValidationErrors | null => {
  const value = control.value as string;
  const errors: Errors = {
    uppercase: 'Uppercase character is required',
    lowercase: 'Lowercase character is required',
    digit: 'Digit is required',
    nonAlpha: 'Non-alphanumeric character is required',
    length: `At least ${minLength} characters are required`,
  };

  if (value.length >= minLength) {
    errors.length = null;
  }

  for (let i = 0; i < value.length; i++) {
    const c = value.charAt(i);

    if (isUppercase(c)) {
      errors.uppercase = null;
    } else if (isLowercase(c)) {
      errors.lowercase = null;
    } else if (isDigit(c)) {
      errors.digit = null;
    } else {
      errors.nonAlpha = null;
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
