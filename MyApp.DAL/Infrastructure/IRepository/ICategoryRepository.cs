using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL.Infrastructure.IRepository
{
    public interface ICategoryRepository: IRepository<Category>
    {
     void Update(Category category);
    }
}
