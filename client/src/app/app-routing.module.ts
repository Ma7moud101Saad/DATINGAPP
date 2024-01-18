import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { TestErrorComponent } from './test-error/test-error.component';
import { NotFoundComponent } from './_error/not-found/not-found.component';
import { ServerErrorComponent } from './_error/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberDetaildResolver } from './_resolvers/member-detaild.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guards/admin.guard';


const routes: Routes = [
  {path:"",component:HomeComponent},
  {path:"",runGuardsAndResolvers:'always',
  canActivate:[AuthGuard],
  children:[
    {path:"members",component:MemberListComponent},
    {path:"member/:userName",component:MemberDetailComponent, resolve: { member: MemberDetaildResolver}},
    {path:"messages",component:MessagesComponent},
    {path:"lists",component:ListsComponent},
    {path:"members/edit",component:MemberEditComponent,canDeactivate: [PreventUnsavedChangesGuard]},
    {path:"admin",component:AdminPanelComponent,canActivate:[AdminGuard]}
  ]},
  {path:"not-found",component:NotFoundComponent},
  {path:"server-error",component:ServerErrorComponent},
  {path:"testError",component:TestErrorComponent},
  {path:"**",component:HomeComponent,pathMatch:'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
