import { Component, OnInit } from '@angular/core';
import { error } from 'console';
import { Observable, observable, of } from 'rxjs';
import { User } from 'src/_models/User';
import { AccountService } from 'src/_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model:any={};
  constructor(public accountService:AccountService) { }

  ngOnInit(): void {
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next:response=>{
        console.log(response);
      },
      error:error=>{
        console.log(error);
      }

    })
  }

  logout(){
    this.accountService.logout();
  }

}
