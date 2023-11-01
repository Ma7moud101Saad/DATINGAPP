import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registermodel=false;
  users:any;
  constructor(private http:HttpClient) { }

  ngOnInit(): void {
    this.getUsers();
  }
  registerToggel(){
    this.registermodel=!this.registermodel;
  }

  getUsers(){
    this.http.get("https://localhost:5001/api/Useres").subscribe({
      next:response=>{this.users=response},
      error:error=>{console.log(error)},
      complete:()=>{console.log("complete")}
     })
  }

  cancelRegisterMode(event:boolean){
    this.registermodel=event;
  }
}
