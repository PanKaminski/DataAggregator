export interface ApiTaskItem{
    id: number;
    name: string;
    description?: string;
    cronExpression: string;
}