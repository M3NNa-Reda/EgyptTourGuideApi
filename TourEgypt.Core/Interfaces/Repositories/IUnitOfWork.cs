using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPlaceRepository Places { get; }
        IGenericRepository<UserCategory> UserInterests { get; }

        Task<int> CompleteAsync(); 
    }
}
