using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL.Infrastructure.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
           //var categoryDb= _context.Categories.FirstOrDefault(x=>x.Id==category.Id);
           // if(categoryDb != null)
           // {
           //     categoryDb.Name = category.Name;
           //     categoryDb.DisplayOrder = category.DisplayOrder;
           // }
        }
    }
}
