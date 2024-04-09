﻿namespace Brix.Models
{
    public class EFLegostoreRepository : ILegoStoreRepository
    {
        private LegostoreContext _context;

        public EFLegostoreRepository(LegostoreContext temp) 
        { 
            _context = temp;
        }

        public IQueryable<Product> Legos => _context.Legos;
    }
}