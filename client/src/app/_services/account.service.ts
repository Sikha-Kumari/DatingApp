import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map} from 'rxjs/operators'
import { User } from '../_models/user';
import { ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  constructor( private http: HttpClient ) { }

  login(model: any){
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((respone: User) => {
        const user = respone;
        if(user)
        {
          this.setCurrentuser(user);
        }
      })
    );

  }
  register(model:any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user:User) => {
        if(user)
        {
          this.setCurrentuser(user);
        }
      })
    );

  }

  setCurrentuser(user: User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);

  }

  logout(){
    localStorage.removeItem('user');
  }


}
