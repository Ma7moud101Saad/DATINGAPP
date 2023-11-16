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
  }
  registerToggel(){
    this.registermodel=!this.registermodel;
  }

  cancelRegisterMode(event:boolean){
    this.registermodel=event;
  }
}
