﻿using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IDataManager
    {
        Task ForwardDataForUserAsync(User user);
    }
}
