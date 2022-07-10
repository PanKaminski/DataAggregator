import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiTasksService } from '../_services/api-tasks.service';
import { ApiType } from '../_models/api.type';
import { first } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-task-create',
  templateUrl: './task-create.component.html',
  styleUrls: ['./task-create.component.css']
})
export class TaskCreateComponent implements OnInit {
  
  apiTaskForm: FormGroup;
  apiTypeNames: any;

  loading = false;
  submitted = false;
  error = '';

  @ViewChild("covid", {static: true}) covidSection!: ElementRef;
  @ViewChild("region", {static: true}) regionSection!: ElementRef;
  @ViewChild("sparkLine", {static: true}) sparkLineSection!: ElementRef;
  @ViewChild("currency", {static: true}) currencySection!: ElementRef;
  
  constructor(
    private formBuilder: FormBuilder, 
    private apiTasksService: ApiTasksService,
    private router: Router) {
      this.apiTypeNames = Object.values(ApiType);

      this.apiTaskForm = this.formBuilder.group({
        name: ['', [Validators.required]],
        description: [''],
        cronTimeExpression: ['* * * * *', Validators.required],
        apiAggregatorType: ['', Validators.required],
        sparkLineTime: [''],
        referenceCurrency: [''],
        region: [''],
        country: [''],
    });
    
  }

  ngOnInit(): void {
  }

  selectApi(){
    var selectedApi = this.apiTaskForm.controls['apiAggregatorType'].value;

    if(selectedApi === ApiType.WeatherTracker){
      this.regionSection.nativeElement.classList.remove('d-none');
      this.covidSection.nativeElement.classList.add('d-none');
      this.sparkLineSection.nativeElement.classList.add('d-none');
      this.currencySection.nativeElement.classList.add('d-none');
    }else if(selectedApi === ApiType.CovidTracker){
      this.regionSection.nativeElement.classList.add('d-none');
      this.covidSection.nativeElement.classList.remove('d-none');
      this.sparkLineSection.nativeElement.classList.add('d-none');
      this.currencySection.nativeElement.classList.add('d-none');
    }else{
      this.regionSection.nativeElement.classList.add('d-none');
      this.covidSection.nativeElement.classList.add('d-none');
      this.sparkLineSection.nativeElement.classList.remove('d-none');
      this.currencySection.nativeElement.classList.remove('d-none');
    }
  }

  setCron(cron: string){
    this.apiTaskForm.patchValue({
      cronTimeExpression: cron
    });
  }

  create(){
    this.submitted = true;

    if (this.apiTaskForm.invalid) {
        return;
    }

    this.loading = true;

    this.apiTasksService.createTask(this.apiTaskForm.value)
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

  get f() { return this.apiTaskForm.controls; }
}
