import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/_models/member';
import { Pagination } from 'src/_models/pagination';
import { User } from 'src/_models/user';
import { UserParams } from 'src/_models/userParams';
import { AccountService } from 'src/_services/account.service';
import { MemberService } from 'src/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
 // members$:Observable<Member[]>|undefined;
  members:Member[]=[];
  pagination:Pagination={
    currentPage: 0,
    itemPerPage: 0,
    totalItems: 0,
    totalPages: 0
  } ;
  userParams:UserParams ={
    pageNumber: 0,
    pageSize: 0,
    minAge: 0,
    maxAge: 0,
    gender: '',
    orderBy: ''
  };
  
  genderList=[{value:'male',name:'male'},{value:'female',name:'female'}];
  constructor(private memberService:MemberService) { 
   this.userParams= this.memberService.getUserParams();
  }

  ngOnInit(): void {
    //this.members$=this.memberService.getMembers();
    this.loadMembers();
  }
  loadMembers(){
    this.memberService.setUserParams(this.userParams);
    this.memberService.getMembers(this.userParams).subscribe(
      {
        next:(resposnse: { result: Member[]; pagination: Pagination; })=>{
          if(resposnse.result && resposnse.pagination){
            this.members=resposnse.result;
            this.pagination=resposnse.pagination;
          }
        }
      })
    }

    pageChanged(event:any){
      this.userParams.pageNumber=event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }

    resetFilters(){
        this.userParams=this.memberService.resetUserParams();
    }
}
