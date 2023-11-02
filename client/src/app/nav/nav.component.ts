import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { error } from 'console';
import { ToastrService } from 'ngx-toastr';
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
  constructor(
    public accountService:AccountService,
    private router:Router,
    private tostar:ToastrService) { }

  ngOnInit(): void {
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next:_=>this.router.navigateByUrl("/members"),
      error:error=>this.tostar.error(error.error)
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl("/");
  }

}
