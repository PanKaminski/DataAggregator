import { Role } from "./role";

export interface UserStatistics {
    id: number;
    email: string;
    role: Role;
    countOfRequests: number;
    registrationDate: string;
    requestsPerDay: number;
}