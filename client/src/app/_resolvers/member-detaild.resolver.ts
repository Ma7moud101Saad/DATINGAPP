import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from 'src/_models/member';
import { MemberService } from 'src/_services/member.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetaildResolver implements Resolve<Member> {
  constructor(private memberService :MemberService) {
  }
  resolve(
    route: ActivatedRouteSnapshot
  ): Observable<Member>|Promise<Member>|Member {
      return this.memberService.getMember(route.paramMap.get("userName")!);
  }
}
