using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DAL.Infrastructure.IRepository;

using MyApp.Models;
using MyApp.Models.ViewModel;
using System.Security.Claims;

namespace EntityFrameworkMvc.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private  IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Cart> ListOfCart { get; private set; }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            CartView itemList = new CartView();
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product");
            };
            return View(itemList);
        }
    }
}
