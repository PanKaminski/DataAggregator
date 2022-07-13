import { Component, OnInit } from '@angular/core';
import { ApiTasksService } from '../_services/api-tasks.service';
import { ApiTaskItem } from '../_models/api-task-item';
import { first } from 'rxjs';

@Component({
  selector: 'app-tasklist',
  templateUrl: './tasklist.component.html',
  styleUrls: ['./tasklist.component.css']
})
export class TasklistComponent implements OnInit {
  loading = false;
  userTasks: ApiTaskItem[] =[];
  constructor(private apiTasksService: ApiTasksService) { }

  ngOnInit(): void {
    this.loading = true;
    this.apiTasksService
      .getCurrentUserTasks()
      .pipe(first())
      .subscribe((userTasks: ApiTaskItem[]) => {
        this.loading = false;
        this.userTasks = userTasks;
    });
  }

  deleteTask(taskId: number){
    console.log('delete');
    this.apiTasksService.deleteTask(taskId).subscribe(() => {
      this.userTasks.splice(this.userTasks.findIndex(ut => ut.id == taskId), 1);
    });
  }
}
