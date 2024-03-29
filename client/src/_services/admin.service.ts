import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from 'src/_models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl=environment.apiUrl;
  constructor(private http:HttpClient) { }

  usersWithRoles(){
    return this.http.get<User[]>(this.baseUrl+"admin/users-with-roles")
  }

  updateUserRoles(userName:string,roles:string[]){
    return this.http.post<string[]>(this.baseUrl+"Admin/edit-roles/"+userName+"?roles="+roles,{});
  }
}
