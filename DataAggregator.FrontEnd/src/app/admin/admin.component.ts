import { Component, OnInit } from '@angular/core';
import { UserService } from '../_services/user.service';
import { UserStatistics } from '../_models/user.statistics';
import { first } from 'rxjs';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  loading = false;
  users: UserStatistics[] = [];

  constructor(private userService: UserService) { }

  ngOnInit() {
      this.loading = true;
      this.userService
        .getAll()
        .pipe(first())
        .subscribe((users: UserStatistics[]) => {
          this.loading = false;
          this.users = users;
      });
  }
}
