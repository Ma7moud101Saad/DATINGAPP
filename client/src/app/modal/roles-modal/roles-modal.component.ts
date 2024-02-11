import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  userName='';
  availableRoles:any[]=[];
  selectedRoles:any[]=[];
  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }
  updateChecked(role:string){
   var index= this.selectedRoles.indexOf(role);
   index !== -1 ?this.selectedRoles.splice(index,1):this.selectedRoles.push(role);
  }

}
