import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';


@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
 
  canDeactivate(component: MemberEditComponent): boolean {
    if (component.editForm.dirty) {
      return confirm("Are u sure u want to leave, the changes are not saved yet!"); 
     }
      return true;
  }
  
}
