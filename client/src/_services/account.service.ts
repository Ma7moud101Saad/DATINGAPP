import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from 'src/_models/User';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl="https://localhost:5001/api/";

  private currentUserSource=new BehaviorSubject<User|null>(null);
  currentUser$=this.currentUserSource.asObservable();
  
  constructor(private http:HttpClient) { }

  login(model:any){
   return this.http.post<User>(this.baseUrl+"account/login",model).pipe(
    map((respose:User)=>{
      const user=respose;
      if(user != null){
        localStorage.setItem("user",JSON.stringify(user));
        this.currentUserSource.next(user);
      }
    })
   );
  }

  register(model:any){
    return this.http.post<User>(this.baseUrl+"account/register",model).pipe(
     map((respose:User)=>{
       const user=respose;
       if(user != null){
         localStorage.setItem("user",JSON.stringify(user));
         this.currentUserSource.next(user);
       }
     })
    );
   }

  logout(){
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
  }

  serCurrentUser(user:User){
    this.currentUserSource.next(user);
  }

}


