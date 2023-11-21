import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { pipe, take } from 'rxjs';
import { User } from 'src/_models/User';
import { Member } from 'src/_models/member';
import { AccountService } from 'src/_services/account.service';
import { MemberService } from 'src/_services/member.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user:User |null=null;
  member: Member={
    id: 0,
    userName: '',
    photoUrl: '',
    age: 0,
    dateOfBirth: '',
    knownAs: '',
    created: '',
    lastActive: '',
    gender: '',
    introduction: '',
    lookingFor: '',
    interstes: undefined,
    city: '',
    country: '',
    photos: []
  };
  @ViewChild('editForm')editForm:NgForm|undefined;

  @HostListener('window:beforeunload',['$event']) unloadNotification($event:any){
    if(this.editForm?.dirty){
      $event.returnValue=true;
    }
  }
  constructor(private accountService:AccountService,private memberService:MemberService,
    private totstarService:ToastrService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=>this.user=user
    })
  }

  ngOnInit(): void {
    
    this.loadMember();
  }

  loadMember(){
    if(!this.user)return;
    this.memberService.getMember(this.user.userName).subscribe({
      next:member=>this.member=member
    })
  }

  editProfile(){
    this.memberService.UpdateMember(this.member).subscribe({
      next:_=>{
        this.totstarService.success("data saved successfuly");
        this.editForm?.reset(this.member);
      }
    })
  }

}
