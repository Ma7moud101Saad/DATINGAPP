import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { error } from 'console';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  users:any;
  constructor(private http:HttpClient ){}
  ngOnInit(): void {
   this.http.get("https://localhost:44308/api/Useres").subscribe({
    next:response=>{this.users=response},
    error:error=>{console.log(error)},
    complete:()=>{console.log("complete")}
   })
  }
  title = 'Dating App';
}
