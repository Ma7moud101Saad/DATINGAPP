import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, map } from 'rxjs';
import { AccountService } from 'src/_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private accountService:AccountService,private tostar:ToastrService) {
  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      return  this.accountService.currentUser$.pipe(
        map(user=>{
          if(!user)return false;
          if(user.roles.includes("Admin") || user.roles.includes("Moderator") ){
            return true;
          }else{
            this.tostar.error("You can not enter this area");
            return false;
          }
        })
      )
  }
  
}
