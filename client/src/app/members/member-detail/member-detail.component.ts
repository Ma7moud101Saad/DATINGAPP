import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs';
import { Member } from 'src/_models/member';
import { Message } from 'src/_models/message';
import { User } from 'src/_models/user';
import { AccountService } from 'src/_services/account.service';
import { MemberService } from 'src/_services/member.service';
import { MessageService } from 'src/_services/message.service';
import { PresenceService } from 'src/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit,OnDestroy{
  member:Member ={} as Member;
  galleryOptions: NgxGalleryOptions[]=[] ;
  galleryImages: NgxGalleryImage[]=[] ;
  @ViewChild('memberTabs',{static:true})memberTabs?:TabsetComponent;
  activeTab?:TabDirective;
  user:User |null=null;
  constructor(
    private accountService:AccountService,
    private route:ActivatedRoute,
    private messageService:MessageService,
    public presenceService:PresenceService) { 
       this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=>this.user=user
    })}
  ngOnDestroy(): void {
    this.messageService.stopHupConnection();
  }

  ngOnInit(): void {
    this.route.data.subscribe({
      next:data=>{
        this.member=data['member'];
      }
    })
    
    this.galleryImages= this.getImages();

    this.route.queryParams.subscribe({
      next:params=>{
        params['tab'] && this.selectTab(params['tab'])
      }
    })

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

  onTabActivate(data:TabDirective){
    this.activeTab=data;
    if(this.activeTab.heading=="Messages" && this.user){
      this.messageService.createHubConnection(this.user,this.member.userName);
    }else{
      this.messageService.stopHupConnection();
    }
  }

  selectTab(heading:string){
    if(this.memberTabs){
      this.memberTabs.tabs.find(x=>x.heading == heading)!.active = true;
    }
  }
}
