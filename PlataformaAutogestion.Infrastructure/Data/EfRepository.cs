using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class EfRepository<T> : BaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public EfRepository(ApplicationDbContext context) : base(context)
        {
            context = _context;
        }
    }
}
