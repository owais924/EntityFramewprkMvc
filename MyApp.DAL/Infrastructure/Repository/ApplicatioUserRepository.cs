using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL.Infrastructure.Repository
{
    public class ApplicatioUserRepository : Repository<ApplicationUser>, IApplicationtUserRepository
    {
        private ApplicationDbContext _context;
        public ApplicatioUserRepository(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }

      
    }
}
