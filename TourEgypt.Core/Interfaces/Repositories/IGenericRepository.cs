using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<IReadOnlyList<T>> GetAllAsync();
    }
}
