import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin/admin.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { TasklistComponent } from './tasklist/tasklist.component';
import { AuthGuard } from './_helpers/auth.guard';
import { Role } from './_models/role';
import { TaskCreateComponent } from './task-create/task-create.component';

const routes: Routes = [
  {
    path: '',
    component: TasklistComponent,
    canActivate: [AuthGuard],
  },
  {
      path: 'new-task',
      component: TaskCreateComponent,
      canActivate: [AuthGuard],
  },
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [AuthGuard],
    data: { roles: [Role.Admin] }
  },
  {
      path: 'login',
      component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },

  // otherwise redirect to home
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
