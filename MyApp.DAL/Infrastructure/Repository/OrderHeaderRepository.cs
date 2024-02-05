using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL.Infrastructure.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
           //var categoryDb= _context.Categories.FirstOrDefault(x=>x.Id==category.Id);
           // if(categoryDb != null)
           // {
           //     categoryDb.Name = category.Name;
           //     categoryDb.DisplayOrder = category.DisplayOrder;
           // }
        }

        public void UpdateStatus(int id, string orderStatus, string? paymenrStatus = null)
        {
           var order= _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (order != null)
            {
                    order.OrderStatus = orderStatus;
            }
            if(paymenrStatus != null)
            {
                order.PayementStatus= paymenrStatus;
            }
        }
    }
}
