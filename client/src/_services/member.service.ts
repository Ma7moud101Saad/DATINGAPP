import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, of, take } from 'rxjs';
import { Member } from 'src/_models/member';
import { PaginationResult } from 'src/_models/pagination';
import { UserParams } from 'src/_models/userParams';
import { environment } from 'src/environments/environment';
import { AccountService } from './account.service';
import { User } from 'src/_models/user';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl=environment.apiUrl;
  members:Member[]=[];
  paginationResult:PaginationResult<Member[]>=new PaginationResult<Member[]>();
  memberCash=new Map();
  user:User ={
    userName: '',
    token: '',
    photoUrl: '',
    gender: ''
  };
  userParams:UserParams ={
    pageNumber: 0,
    pageSize: 0,
    minAge: 0,
    maxAge: 0,
    gender: '',
    orderBy: ''
  };
  constructor(private http:HttpClient,private accountService:AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=>{
        if(user)
        {
          this.user=user;
          this.userParams=new UserParams(user)
        }
      }
    })
  }

  getMembers(userParams:UserParams):any{
    const response=this.memberCash.get(Object.values(userParams).join("-"));
    if(response) return of(response);
    let params= this.getPagginationHeaders(userParams.pageNumber,userParams.pageSize);
    params=params.append("minAge",userParams.minAge);
    params=params.append("maxAge",userParams.maxAge);
    params=params.append("gender",userParams.gender);
    params=params.append("orderBy",userParams.orderBy);
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params).pipe(
      map(response=>{
        this.memberCash.set(Object.values(userParams).join("-"),response);
        return response;
      })
    );
  }

  getPaginatedResult<T>(url:string,params:HttpParams){
    const  paginationResult:PaginationResult<T>=new PaginationResult<T>();
    return this.http.get<T>(url,{observe:'response',params}).pipe(
      map(
        response=>{
          if(response.body){
            paginationResult.result=response.body
          }
          const pagination=response.headers.get('Pagination');
          
          if(pagination){
            paginationResult.pagination=JSON.parse(pagination);
          }
          return paginationResult;
        } 
      ))
  }

  getPagginationHeaders(pageNumber:number,pageSize:number){
    let params=new HttpParams();
      params=params.append("pageNumber",pageNumber);
      params=params.append("pageSize",pageSize);
      return params;
  }

  getMember(userName:string){

    var member=[...this.memberCash.values()].
    reduce((arr,elem) => arr.concat(elem.result) , []).
    find((member:Member)=>member.userName==userName);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl+"users/"+userName);
  }

  UpdateMember(member:Member){
    return this.http.put(this.baseUrl+"users/",member).pipe(map(
      _=>{
       const index= this.members.indexOf(member!);
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

  addLike(userName:string){
    return this.http.post(this.baseUrl+"likes/"+userName,{});
  }
  getUserLike(predicate:string,pageNumber:number,pageSize:number){
    let params= this.getPagginationHeaders(pageNumber,pageSize);
    params=params.append("predicate",predicate);
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'likes', params).pipe(
      map(response=>{
        return response;
      })
    );
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

  setUserParams(params:UserParams){
    this.userParams=params;
  }
  getUserParams(){
    return this.userParams;
  }
  resetUserParams(){
    return new UserParams(this.user); 
  }

}
