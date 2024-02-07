﻿using MyApp.DAL.Infrastructure.IRepository;
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

        public void PaymentStatus(int Id, string SessionId, string PaymentIntentId)
        {
           var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            orderHeader.DateOfPayment= DateTime.Now;
            orderHeader.PaymentIntentId = PaymentIntentId;
            orderHeader.SessionId = SessionId;
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

        public void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null)
        {
           var order= _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            if (order != null)
            {
                    order.OrderStatus = orderStatus;
            }
            if(paymentStatus != null)
            {
                order.PayementStatus= paymentStatus;
            }
        }
    }
}
