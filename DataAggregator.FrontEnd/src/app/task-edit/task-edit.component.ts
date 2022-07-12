import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiTasksService } from '../_services/api-tasks.service';
import { ApiTaskModel } from '../_models/api-task.model';
import { first } from 'rxjs';
import { ApiType } from '../_models/api.type';

@Component({
  selector: 'app-task-edit',
  templateUrl: './task-edit.component.html',
  styleUrls: ['./task-edit.component.css']
})
export class TaskEditComponent implements OnInit, AfterViewInit {

  id: number;
  apiTaskForm!: FormGroup;
  userTask: ApiTaskModel = {
    name: '',
    description: '',
    cronTimeExpression: ''
  };

  loading = false;
  submitted = false;
  error = '';

  @ViewChild("covid", {static: true}) covidSection!: ElementRef;
  @ViewChild("region", {static: true}) regionSection!: ElementRef;
  @ViewChild("sparkLine", {static: true}) sparkLineSection!: ElementRef;
  @ViewChild("currency", {static: true}) currencySection!: ElementRef;

  constructor(private formBuilder: FormBuilder, 
    private apiTasksService: ApiTasksService,
    private router: Router,
    private activateRoute: ActivatedRoute){
      this.id = activateRoute.snapshot.params['id'];

      this.apiTasksService
      .getTask(this.id)
      .pipe(first())
      .subscribe((userTask: ApiTaskModel) => {
        this.userTask = userTask;
        this.userTask.apiAggregatorType = this.userTask.region ? 
        ApiType.WeatherTracker :this.userTask.country ? 
        ApiType.CovidTracker : ApiType.CurrencyTracker;

        this.apiTaskForm = this.formBuilder.group({
          name: [this.userTask.name, [Validators.required]],
          description: [this.userTask.description],
          cronTimeExpression: [this.userTask.cronTimeExpression, Validators.required],
          apiAggregatorType: [this.userTask.apiAggregatorType, Validators.required],
          sparkLineTime: [this.userTask.sparkLineTime ? this.userTask.sparkLineTime : ''],
          referenceCurrency: [this.userTask.referenceCurrency ? this.userTask.referenceCurrency : ''],
          region: [this.userTask.region ? this.userTask.region : ''],
          country: [this.userTask.country ? this.userTask.country : ''],
      });
    });
  }
  ngAfterViewInit(): void {
    this.selectApi();
  }

  ngOnInit(): void {
  }

  get f() { return this.apiTaskForm.controls; }

  update(){
    this.submitted = true;

    if (this.apiTaskForm.invalid) {
        return;
    }

    this.loading = true;

    this.apiTasksService.updateTask(this.id, this.apiTaskForm.value)
        .pipe(first())
        .subscribe({
            next: () => {
                this.router.navigateByUrl('/');
            },
            error: error => {
                this.error = error;
                this.loading = false;
            }
        });
  }

  setCron(cron: string){
    this.apiTaskForm.patchValue({
      cronTimeExpression: cron
    });
  }

  private selectApi(){
    if(this.userTask.apiAggregatorType === ApiType.WeatherTracker){
      this.regionSection.nativeElement.classList.remove('d-none');
    }else if(this.userTask.apiAggregatorType === ApiType.CovidTracker){
      this.covidSection.nativeElement.classList.remove('d-none');
    }else{
      this.sparkLineSection.nativeElement.classList.remove('d-none');
      this.currencySection.nativeElement.classList.remove('d-none');
    }
  }
}
