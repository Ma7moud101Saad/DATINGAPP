import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserParams } from 'src/_models/userParams';
import { getPagginationHeaders, getPaginatedResult } from './paginationHelper';
import { Message } from 'src/_models/message';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl=environment.apiUrl;
  constructor(private http:HttpClient) { }

  getMessages(pageNumber:number,pageSize:number,container:string):any{
    let params= getPagginationHeaders(pageNumber,pageSize);
    params=params.append("container",container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params,this.http).pipe(
      map(response=>{
        return response;
      })
    );
  }

  getMessagesThread(userName:string):any{
    return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+userName);
  }

  sendMessage(userName:string,content:string):any{
   return this.http.post(this.baseUrl+'Messages',{recipientUserName:userName,content:content});
  }
  

  deleteMessage(id:number){
   return this.http.delete(this.baseUrl+"Messages/"+id);
  }
}
