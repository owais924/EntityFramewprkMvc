using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL.Infrastructure.IRepository
{
    public interface IOrderHeaderRepository: IRepository<OrderHeader>
    {
     void Update(OrderHeader orderHeader);
        void UpdateStatus(int id, string orderStatus, string? paymenrStatus=null);
    }
}
