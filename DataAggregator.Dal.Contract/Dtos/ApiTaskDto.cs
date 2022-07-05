﻿namespace DataAggregator.Dal.Contract.Dtos
{
    public class ApiTaskDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AggregatorApiDto Api { get; set; }

        public UserDto Subscriber { get; set; }

        public string CronTimeExpression { get; set; }
    }
}
