import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, map, Observable } from "rxjs";

import { environment } from "src/environments/environment";
import { User } from "../_models";

@Injectable({providedIn: 'root'})
export class AuthenticationService{
    private userSubject: BehaviorSubject<User | null>;
    public user: Observable<User | null>;

    constructor(
        private router: Router,
        private http: HttpClient
    ) {
        this.userSubject = new BehaviorSubject<User| null>(JSON.parse(localStorage.getItem('user') || '{}'));

        if(!this.isUser(this.userValue)){
            this.userSubject.next(null);
        }

        this.user = this.userSubject.asObservable();
    }

    public get userValue(): User | null {
        return this.userSubject.value;
    }

    register(email: string, password: string, confirmPassword: string) {
        return this.http.post<any>(`${environment.apiUrl}/account/register`, { email, password, confirmPassword })
            .pipe(map((user: User) => {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('user', JSON.stringify(user));
                this.userSubject.next(user);
                return user;
            }));
    }

    login(email: string, password: string) {
        return this.http.post<any>(`${environment.apiUrl}/account/login`, { email, password })
            .pipe(map((user: User) => {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('user', JSON.stringify(user));
                this.userSubject.next(user);
                return user;
            }));
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('user');
        this.userSubject.next(null);
        this.router.navigate(['/login']);
    }

    private isUser(object: any): object is User {
        return "email" in object && "password" in object && "role" in object && "token" in object;
      }
}