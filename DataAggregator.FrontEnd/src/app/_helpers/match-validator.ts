import { AbstractControl, ValidatorFn } from '@angular/forms';

export function matchContent(expected: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control.parent?.get(expected)?.value 
        ? null : {isMatching: true}
    }
  }