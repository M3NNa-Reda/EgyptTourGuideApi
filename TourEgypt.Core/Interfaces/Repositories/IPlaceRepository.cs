using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface IPlaceRepository:IGenericRepository<Place>
    {


        Task<IReadOnlyList<Place>> GetByCategoryAsync(int categoryId, int page, int pageSize);
        Task<IReadOnlyList<Place>> SearchAsync(string keyword, int count);

        Task<IReadOnlyList<Place>> GetNearbyAsync(double latitude, double longitude, double maxDistanceInKm = 10);


        Task<IReadOnlyList<Place>> GetAllPlacesWithReviewsAsync();

        Task<IReadOnlyList<Place>> GetTopPlacesAsync(int page, int pageSize);
        Task<IReadOnlyList<Place>> GetByCityAsync(int cityId, int page, int pageSize);
        


    }
}
