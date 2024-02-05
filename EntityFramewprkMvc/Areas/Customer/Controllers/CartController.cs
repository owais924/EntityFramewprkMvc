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
        private readonly IUnitOfWork _unitOfWork;
       // public CartView vm { get; set; }
        public CartController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToAction("Index", "Home"); // Or any appropriate action
            }

            var vm = new CartView
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new MyApp.Models.OrderHeader()
            };
          

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal = vm.ListOfCart.Sum(item => item.Product.Price * item.Count);
            }
            return View(vm);
        }
        public IActionResult Summery() 
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToAction("Index", "Home"); // Or any appropriate action
            }

            var vm = new CartView
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new MyApp.Models.OrderHeader()
            };
            vm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == claim.Value);
            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;
            vm.OrderHeader.Phone = vm.OrderHeader.ApplicationUser.PhoneNumber;
            vm.OrderHeader.Address = vm.OrderHeader.ApplicationUser.Address;
            vm.OrderHeader.City = vm.OrderHeader.ApplicationUser.City;
            vm.OrderHeader.State = vm.OrderHeader.ApplicationUser.State;
            vm.OrderHeader.PostalCode = vm.OrderHeader.ApplicationUser.PinCode;

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal = vm.ListOfCart.Sum(item => item.Product.Price * item.Count);
            }
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summery(CartView vm) 
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            vm.ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");
            return View();
        }
        public IActionResult plus(int id) 
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);

            _unitOfWork.Cart.IncrementCartItem(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));       
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            if (cart.Count<= 1)
            {
                _unitOfWork.Cart.Delete(cart);
                
            }
            else
            {
            _unitOfWork.Cart.DecrementCartItem(cart, 1);

            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            _unitOfWork.Cart.Delete(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        //public IEnumerable<Cart> ListOfCart { get; private set; }


       
    }
}
