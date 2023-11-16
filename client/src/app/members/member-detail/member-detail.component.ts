import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/_models/member';
import { AccountService } from 'src/_services/account.service';
import { MemberService } from 'src/_services/member.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member:Member | undefined
  galleryOptions: NgxGalleryOptions[]=[] ;
  galleryImages: NgxGalleryImage[]=[] ;
  constructor(private memberService:MemberService,private route:ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent:100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }
    ];
  }

  loadMember(){
    var userName=this.route.snapshot.paramMap.get("userName");
    if(!userName)return;
    this.memberService.getMember(userName).subscribe({
      next:member=>{
        this.member=member;
       this.galleryImages= this.getImages();
      }
    })
  }

  getImages(){
    if(!this.member) return [];
    const galleryImages:NgxGalleryImage[]=[] 
    for(let photo of this.member.photos){
      galleryImages.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url
      })
    }
    return galleryImages;
  }

}
