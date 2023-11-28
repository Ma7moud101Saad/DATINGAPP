import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, of } from 'rxjs';
import { Member } from 'src/_models/member';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl=environment.apiUrl;
  members:Member[]=[];
  constructor(private http:HttpClient) { }

  getMembers(){
    if(this.members.length > 0)return of(this.members);
    return this.http.get<Member[]>(this.baseUrl+"users").pipe(
      map(members=>{
        this.members=members;
        return members;
      }))
  }

  getMember(userName:string){
    const member= this.members.find(x=>x.userName===userName);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl+"users/"+userName);
  }

  UpdateMember(member:Member){
    return this.http.put(this.baseUrl+"users/",member).pipe(map(
      _=>{
       const index= this.members.indexOf(member);
       this.members[index]={...this.members[index],...member}
      }
    ));
  }

  setMainPhoto(photoId:number){
    return this.http.put(this.baseUrl+"users/set-main-photo/"+photoId,{});
  }

  deletePhoto(photoId:number){
    return this.http.delete(this.baseUrl+"users/delete-photo/"+photoId);
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
