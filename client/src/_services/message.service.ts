import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserParams } from 'src/_models/userParams';
import { getPagginationHeaders, getPaginatedResult } from './paginationHelper';
import { Message } from 'src/_models/message';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, map, take } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from 'src/_models/user';
import { Group } from 'src/_models/group';
@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl=environment.apiUrl;
  hupUrl=environment.hupUrl;
  private hubConnection?:HubConnection;
  private messageThreadSource=new BehaviorSubject<Message[]>([]);
  messageThread$=this.messageThreadSource.asObservable();
  constructor(private http:HttpClient) { }
  
  createHubConnection(user:User,otherUserName:string){
    this.hubConnection=new HubConnectionBuilder()
    .withUrl(this.hupUrl+'/message?user='+otherUserName,{
      accessTokenFactory:()=>user.token
    })
    .withAutomaticReconnect()
    .build();
    this.hubConnection.start().catch(error=>console.log(error));

    this.hubConnection.on('ReceveMessageThread',messages=>{
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('UpdatedGroup',(group:Group)=>{
      if(group.connections.some(c=>c.userName == user.userName))
      this.messageThread$.pipe(take(1)).subscribe({next:messages=>{
        messages.forEach(message => {
          if(!message.dateRead)
            message.dateRead = new Date(Date.now());
        });
        this.messageThreadSource.next([...messages]);
      }})
    });

    this.hubConnection.on('NewMessage',message=>{
      this.messageThread$.pipe(take(1)).subscribe({next:messages=>{
        this.messageThreadSource.next([...messages,message]);
      }})
    });
  }
  stopHupConnection(){
    this.hubConnection?.stop().catch(error=>console.log(error));
  }
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
    return this.hubConnection?.invoke('SendMessage',{recipientUserName:userName,content:content})
    .catch(error=>console.log(error));
  }
  

  deleteMessage(id:number){
   return this.http.delete(this.baseUrl+"Messages/"+id);
  }
}
