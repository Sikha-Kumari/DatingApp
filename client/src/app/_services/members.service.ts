import { Injectable } from '@angular/core';
import { HttpClient,  HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
 
  baseURL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMembers(){
      return this.http.get<Member[]>(this.baseURL + 'users');
  }

  getMember(username : string){
    return this.http.get<Member>(this.baseURL + 'users/' + username);
}

}
