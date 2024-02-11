import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/_models/message';
import { MessageService } from 'src/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input()userName?: string;
  messageContent:string ='';
  @ViewChild('sendMessageForm')sendMessageForm:NgForm|undefined;
  constructor(public messageService:MessageService) { }

  ngOnInit(): void {
  }

 
  sendMessage(){
    if(!this.userName)return;
    this.messageService.sendMessage(this.userName,this.messageContent).then(()=>{
      this.sendMessageForm?.reset();
    })
  }

}
