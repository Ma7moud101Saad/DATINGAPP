import { Component, OnInit } from '@angular/core';
import { Member } from 'src/_models/member';
import { Pagination } from 'src/_models/pagination';
import { MemberService } from 'src/_services/member.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  predicate:string="liked"
  members:Member[]=[];
  pagination:Pagination={
    currentPage: 0,
    itemPerPage: 0,
    totalItems: 0,
    totalPages: 0
  } ;
  pageNumber:number=1;
  pageSize:number=5
  constructor(private memberService:MemberService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.memberService.getUserLike(this.predicate,this.pageNumber,this.pageSize).subscribe({
      next:response=>{
        this.members=response.result!;
        this.pagination=response.pagination!
      }
    })
  }


  pageChanged(event:any){
    this.pageNumber=event.page;
    this.loadLikes();
  }
}
