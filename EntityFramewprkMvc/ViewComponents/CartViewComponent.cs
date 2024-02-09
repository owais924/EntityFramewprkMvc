using Microsoft.AspNetCore.Mvc;
using MyApp.DAL.Infrastructure.IRepository;
using System.Security.Claims;

namespace EntityFrameworkMvc.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32("SessionCart")!=null)
                {

                return View(HttpContext.Session.GetInt32("SessionCart"));
                }
                else
                {
                    HttpContext.Session.SetInt32("SessionCart", _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count);
                }
                return View(HttpContext.Session.GetInt32("SessionCart"));
            }
          else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
           
        }
    }
}
