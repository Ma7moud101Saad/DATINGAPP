
import { Component, OnInit } from '@angular/core';
import { Message } from 'src/_models/message';
import { Pagination } from 'src/_models/pagination';
import { MessageService } from 'src/_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  pageNumber:number=1;
  pageSize:number=5;
  container:string="Unread";
  messages:Message[]=[];
  pagination:Pagination={
    currentPage: 0,
    itemPerPage: 0,
    totalItems: 0,
    totalPages: 0
  };
  loading:boolean=true;
  constructor(private messageService:MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading=true;
    this.messageService.getMessages(this.pageNumber,this.pageSize,this.container).subscribe(
      {
        next:(resposnse: { result: Message[]; pagination: Pagination; })=>{
          if(resposnse.result && resposnse.pagination){
            this.messages=resposnse.result;
            this.pagination=resposnse.pagination;
          }
          this.loading=false;
        }
      })
  }

  pageChanged(event:any){
    this.pageNumber=event.page;
    this.loadMessages();
  }

  deleteMessage(id:number){
    this.messageService.deleteMessage(id).subscribe({
      next:()=>{this.messages.splice(this.messages.findIndex(x=>x.id == id),1)}
    })
  }

}
