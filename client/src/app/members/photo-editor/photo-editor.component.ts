import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { User } from 'src/_models/user';
import { Member } from 'src/_models/member';
import { Photo } from 'src/_models/photo';
import { AccountService } from 'src/_services/account.service';
import { MemberService } from 'src/_services/member.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input()member:Member|undefined;
  uploader:FileUploader|undefined;
  hasBaseDropZoneOver=false;
  baseUrl=environment.apiUrl;
  user:User |null=null;
  constructor(private accountService:AccountService,
    private memberService:MemberService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(
      {
        next:user=>this.user=user
      }
    )
   }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e:any){
    this.hasBaseDropZoneOver=e;
  }

  initializeUploader(){
    this.uploader=new FileUploader({
      url:this.baseUrl+'users/add-photo',
      authToken:'Bearer '+this.user?.token,
      isHTML5:true,
      allowedFileType:['image'],
      removeAfterUpload:true,
      autoUpload:false,
      maxFileSize:10 * 1024 * 1024
    })

    this.uploader.onAfterAddingFile=(file)=>{
      file.withCredentials=false
    }

    this.uploader.onSuccessItem=(item,response,status,headers)=>{
      if(response){
        const photo=JSON.parse(response);
        this.member?.photos.push(photo)
        if(this.user&& this.member && photo.isMain){
          this.user.photoUrl=photo.url;
          this.member.photoUrl=photo.url;
          this.accountService.serCurrentUser(this.user);
        }
      }
    }
  }

  setMainPhoto(photo:Photo){
    this.memberService.setMainPhoto(photo.id).subscribe({
      next:()=>{
        if(this.member && this.user){
          this.member.photoUrl=photo.url;
          this.user.photoUrl=photo.url;
          this.accountService.serCurrentUser(this.user);
          this.member.photos.forEach(p=>{
            p.isMain=false;
           if(p.id===photo.id){
            p.isMain=true
           }
          })
        }
      }
    })
  }

  deletePhoto(photoId:number){
    this.memberService.deletePhoto(photoId).subscribe({
      next:_=>{
        if(this.member)
        this.member.photos=this.member.photos.filter(x=>x.id!= photoId);
      }
    })
  }
}
