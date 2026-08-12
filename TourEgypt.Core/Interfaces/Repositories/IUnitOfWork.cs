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

        IFavouriteRepository Favourites { get; }
        ICategoryRepository Categories { get; }
        ICityRepository Cities { get; }
        IReviewRepository Reviews  { get; }



        Task<int> CompleteAsync(); 
    }
}
