using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;

using System.Diagnostics;
using System.Security.Claims;

namespace EntityFrameworkMvc.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitofWork)
        {
            _logger = logger;
            _unitofWork = unitofWork;
        }
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Product> product=_unitofWork.Product.GetAll(includeProperties:"Category");
            return View(product);
        }
        [HttpGet]
        public IActionResult Details(int? ProductId)
        {
            Cart cart = new Cart()
            {
                Product = _unitofWork.Product.GetT(x => x.Id == ProductId, includeProperties: "Category"),
                Count = 1,
                ProductId = (int)ProductId

            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
           
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claims.Value;
                var cartItem = _unitofWork.Cart.GetT(x => x.ProductId == cart.ProductId && x.ApplicationUserId==claims.Value);
                if(cartItem == null) { 
                _unitofWork.Cart.Add(cart);
               
                }
                else
                {
                    _unitofWork.Cart.IncrementCartItem(cartItem, cart.Count);
                }
                    _unitofWork.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
