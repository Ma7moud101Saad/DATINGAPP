import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from 'src/_models/user';
import { environment } from 'src/environments/environment.development';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl=environment.apiUrl;

  private currentUserSource=new BehaviorSubject<User|null>(null);
  currentUser$=this.currentUserSource.asObservable();
  
  constructor(private http:HttpClient) { }

  login(model:any){
   return this.http.post<User>(this.baseUrl+"account/login",model).pipe(
    map((respose:User)=>{
      const user=respose;
      if(user != null){
        this.serCurrentUser(user);
      }
    })
   );
  }

  register(model:any){
    return this.http.post<User>(this.baseUrl+"account/register",model).pipe(
     map((respose:User)=>{
       const user=respose;
       if(user != null){
        this.serCurrentUser(user);
       }
     })
    );
   }

  logout(){
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
  }

  serCurrentUser(user:User){
    user.roles=[];
    const roles=this.getDecodeToken(user.token).role;
    Array.isArray(roles)?user.roles=roles:user.roles.push(roles);
    localStorage.setItem("user",JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  getDecodeToken(token:string){
    return JSON.parse(atob(token.split('.')[1]));
  }
}


