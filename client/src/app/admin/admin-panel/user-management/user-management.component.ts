import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/_models/user';
import { AdminService } from 'src/_services/admin.service';
import { RolesModalComponent } from 'src/app/modal/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users:User[]=[];
  availableRoles:any[]=[
    'Admin',
    'Moderator',
    'Member'
  ];
  bsModalRef: BsModalRef<RolesModalComponent>=new BsModalRef<RolesModalComponent>();
  constructor(private adminService:AdminService,private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.adminService.usersWithRoles().subscribe({
      next:users=>{this.users=users}
    })
  }

  openRolesModal(user:User) {
    const config={
      class:'modal-dialog-centred',
      initialState:{
        userName:user.userName,
        availableRoles:this.availableRoles,
        selectedRoles:[...user.roles]
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent,config);
    this.bsModalRef.onHide?.subscribe({
      next:()=>{
        const selectedRoles=this.bsModalRef.content?.selectedRoles;
        if(!this.arrayEqual(selectedRoles!,user.roles)){
          this.adminService.updateUserRoles(user.userName,selectedRoles!).subscribe(
            {
              next:roles=>user.roles=roles
            }
          )
        }
      }
    })
  }

  arrayEqual(arr1:any[],arr2:any[]){
   return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort())
  }

}
