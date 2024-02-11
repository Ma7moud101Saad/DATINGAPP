import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { error } from 'console';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { User } from 'src/_models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hupUrl=environment.hupUrl;
  private hubConnection?:HubConnection;
  private onlineUsersSource = new BehaviorSubject<any[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();
  constructor(private toastrService:ToastrService,private router:Router) { }

  createHubConnection(user:User){
    this.hubConnection=new HubConnectionBuilder()
    .withUrl(this.hupUrl+'/presence',{
      accessTokenFactory:()=>user.token
    })
    .withAutomaticReconnect()
    .build();
    this.hubConnection.start().catch(error=>console.log(error));

    this.hubConnection.on('UserIsOnline',userName=>{
      this.onlineUsers$.pipe(take(1)).subscribe({
        next:userNames=>this.onlineUsersSource.next([...userNames,userName])
      })
    });

    this.hubConnection.on('UserIsOffline',userName=>{
      this.onlineUsers$.pipe(take(1)).subscribe({
        next:usernams=>this.onlineUsersSource.next(usernams.filter(x=>x!== userName))
      })
    });

    
    this.hubConnection.on('GetOnlineUsers',userNames=>{
      this.onlineUsersSource.next(userNames);
    });
    
    this.hubConnection.on('NewMessageReceived',({userName,knownAs})=>{
      this.toastrService.info(knownAs+" has sent you a new message! Click me to see it")
      .onTap
      .pipe(take(1))
      .subscribe({
        next:()=>this.router.navigateByUrl('/member/'+userName+'?tab=Messages')
      })

    });

  }

  stopHupConnection(){
    this.hubConnection?.stop().catch(error=>console.log(error));
  }
}
