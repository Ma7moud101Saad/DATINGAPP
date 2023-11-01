import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { error } from 'console';
import { AccountService } from 'src/_services/account.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private accountService:AccountService ){}
  ngOnInit(): void {
    this.serCurrentUser();
  }
  title = 'Dating App';

  

  serCurrentUser(){
   var userString=localStorage.getItem("user");
    if( !userString)return
    let user=JSON.parse(userString);
    this.accountService.serCurrentUser(user);
  }
}
