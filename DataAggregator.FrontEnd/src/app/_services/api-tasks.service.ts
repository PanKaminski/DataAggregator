import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { environment } from "src/environments/environment";
import { ApiType } from "../_models";
import { ApiTaskItem } from '../_models/api-task-item';
import { ApiTaskCreateModel } from '../_models/api-task.create.model';

@Injectable({providedIn: 'root'})
export class ApiTasksService{
    private baseUrl = `${environment.apiUrl}/aggregators`;

    constructor(
        private router: Router,
        private http: HttpClient) {
         }

    getCurrentUserTasks() {
        return this.http.get<ApiTaskItem[]>(`${this.baseUrl}/`);
    }

    createTask(taskModel: ApiTaskCreateModel){
        var selectedApiType = taskModel.apiAggregatorType;
        if(selectedApiType === ApiType.CovidTracker){
            return this.http.post<any>(`${this.baseUrl}/covid`, {
                name: taskModel.name,
                description:taskModel.description,
                cronTimeExpression: taskModel.cronTimeExpression,
                api: {
                    country: taskModel.country
                }
            });
        } else if(selectedApiType === ApiType.CurrencyTracker){
            return this.http.post<any>(`${this.baseUrl}/coin`, {
                name: taskModel.name,
                description:taskModel.description,
                cronTimeExpression: taskModel.cronTimeExpression,
                api: {
                    sparkLineTime: taskModel.sparkLineTime,
                    referenceCurrency: taskModel.referenceCurrency    
                }
            });

        } else{
            return this.http.post<any>(`${this.baseUrl}/weather`, {
                name: taskModel.name,
                description:taskModel.description,
                cronTimeExpression: taskModel.cronTimeExpression,
                api: {
                    region: taskModel.region
                }
            });
        }
    }

    deleteTask(taskId: number){
        return this.http.delete(`${this.baseUrl}/${taskId}`);
    }
}