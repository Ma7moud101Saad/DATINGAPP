import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCounter=0;
  constructor(private spinner: NgxSpinnerService) { }
  busy(){
    this.busyRequestCounter++;
    this.spinner.show(undefined,{
      type:'line-scale-party',
      bdColor:'rgba(255,255,255,0)',
      color:'#333333'
    });
  }
  
  idle(){
      this.busyRequestCounter--;
      if(this.busyRequestCounter<=0)
      this.busyRequestCounter=0;
    this.spinner.hide();
  }
}
