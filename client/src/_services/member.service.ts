import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Member } from 'src/_models/member';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl=environment.apiUrl;
  constructor(private http:HttpClient) { }

  getMembers(){
    return this.http.get<Member[]>(this.baseUrl+"users");
  }

  getMember(userName:string){
    return this.http.get<Member>(this.baseUrl+"users/"+userName);
  }

  // getHttpOptions(){
  //   const userString=localStorage.getItem("user");
  //   if(!userString)return;
  //   const user= JSON.parse(userString);
  //   return {
  //     headers:new HttpHeaders({
  //       Authorization:'Bearer '+user.token
  //     })
  //   }
  // }

}
