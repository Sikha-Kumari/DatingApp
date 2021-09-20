import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map} from 'rxjs/operators'
import { User } from '../_models/user';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = ' https://localhost:5001/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  constructor( private http: HttpClient ) { }

  login(model: any){
    return this.http.post<User>(this.baseUrl + 'api/account/login', model).pipe(
      map((respone: User) => {
        const user = respone;
        if(user)
        {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );

  }
  register(model:any){
    return this.http.post<User>(this.baseUrl + 'api/account/register', model).pipe(
      map((user:User) => {
        if(user)
        {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );

  }

  setCurrentuser(user: User){
    this.currentUserSource.next(user);

  }

  logout(){
    localStorage.removeItem('user');
  }


}
