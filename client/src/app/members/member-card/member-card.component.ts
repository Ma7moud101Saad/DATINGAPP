import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/_models/member';
import { MemberService } from 'src/_services/member.service';
import { PresenceService } from 'src/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member:Member | undefined;
  constructor(
    private memberService:MemberService,
    private tostar:ToastrService,
    public presenceService:PresenceService) { }

  ngOnInit(): void {
  }
  addLike(member:Member){
    this.memberService.addLike(member.userName).subscribe({
      next:()=>{this.tostar.success("You have Liked "+member.userName)}
    })
  }
}
