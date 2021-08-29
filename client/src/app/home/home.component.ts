import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;

  constructor() { }

  ngOnInit(): void {
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }

 /*  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(users => {
      this.users = users;
    }, error => {
      console.log(error);
    })

  } */
  cancelRegisterMode(event: boolean){
    this.registerMode = event;
  }
}
