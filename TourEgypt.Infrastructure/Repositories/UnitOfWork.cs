using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Data.Context;


namespace TourEgypt.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IPlaceRepository Places { get; private set; }
        public IGenericRepository<UserCategory> UserInterests { get; private set; }

        public UnitOfWork(AppDbContext context, IPlaceRepository placeRepository)
        {
            _context = context;
            Places = placeRepository;
            UserInterests = new GenericRepository<UserCategory>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
