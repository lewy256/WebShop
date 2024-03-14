
import {ErrorHandler, Injectable} from '@angular/core';
import {ValidationError} from "../api/identity.service";
import {AbstractControl, FormGroup, ValidationErrors, ValidatorFn} from "@angular/forms";

@Injectable({
  providedIn: 'root'
})

export class ErrorMappingService {
  MappingValidationError(errors:ValidationError[],form:FormGroup):void{
    errors.forEach((error: ValidationError)=>{
      let propertyName = error.propertyName as string;
      const controlName=propertyName.replace(/^\w/, c => c.toLowerCase())
      const control = form.get(controlName);
      let messageToReturn = (error.errorMessage?.toLowerCase() || '')
        .replace(/'/g, '')
        .replace(/^\w/, c => c.toUpperCase());
      if (control) {
        control.setErrors({ 'incorrect': messageToReturn});
      }
    })
  }
  uniqueUserNameValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      // Logic for validation
      const isValid = control.value.length <1;

      // Returning validation result
      return isValid ? null : { 'uniqueUserName': { value: control.value } };
    };

  }

  // private setErrorMessage(errors: ValidationErrors) {
  //   if (errors.serverError) {
  //     this.message = errors.serverError;
  //   } else if (errors.required) {
  //     this.message = 'Required field';
  //   } else if (errors.minlength) {
  //     this.message = `Max length is ${errors.minlength.actualLength}/${errors.minlength.requiredLength}`;
  //   } else if (errors.maxlength) {
  //     this.message = `Min length is ${errors.maxlength.actualLength}/${errors.maxlength.requiredLength}`;
  //   } else if (errors.email) {
  //     this.message = 'Email is not valid';
  //   } else if (errors.min) {
  //     this.message = `Min value is ${errors.min.min}, actual value is ${errors.min.actual}`;
  //   } else if (errors.max) {
  //     this.message = `Max value is ${errors.max.max}, actual value is ${errors.max.actual}`;
  //   } else if (errors.pattern) {
  //     this.message = 'Invalid value';
  //   } else if (errors.passwordMismatch) {
  //     this.message = 'Passwords do not match';
  //   } else {
  //     this.message = '';
  //   }
  // }
  //
  // private setErrorToErrorObject(field: string) {
  //   Object.defineProperty(this.errorObject, field, {value: this.message, writable: true});
  // }
}
