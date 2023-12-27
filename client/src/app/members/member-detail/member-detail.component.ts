import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/_models/member';
import { Message } from 'src/_models/message';
import { MemberService } from 'src/_services/member.service';
import { MessageService } from 'src/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member:Member ={} as Member;
  galleryOptions: NgxGalleryOptions[]=[] ;
  galleryImages: NgxGalleryImage[]=[] ;
  @ViewChild('memberTabs',{static:true})memberTabs?:TabsetComponent;
  activeTab?:TabDirective;
  messages:Message[]=[];
  constructor(
    private route:ActivatedRoute,
    private messageService:MessageService) { }

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
    if(this.activeTab.heading=="Messages"){
      this.loadMessagesThread();
    }
  }

  selectTab(heading:string){
    if(this.memberTabs){
      this.memberTabs.tabs.find(x=>x.heading == heading)!.active = true;
    }
  }

  loadMessagesThread(){
    if(this.member){
      this.messageService.getMessagesThread(this.member.userName).subscribe({
        next:(respose: Message[])=>{this.messages=respose;}
      })
    }
  }

}
