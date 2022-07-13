export interface ApiTaskModel{
    name: string;
    description: string;
    cronTimeExpression: string;
    apiAggregatorType?: string;
    sparkLineTime?: string;
    referenceCurrency?: string;
    region?: string;
    country?: string;
}