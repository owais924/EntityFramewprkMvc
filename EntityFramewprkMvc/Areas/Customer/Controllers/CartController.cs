using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyApp.CommonHelper;
using MyApp.DAL.Infrastructure.IRepository;
using Stripe.Checkout;
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
        public CartView vm { get; set; }
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
                return RedirectToAction("Index", "Home"); 
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
        
        public IActionResult Summery(CartView vm) 
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            vm.ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");
            vm.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            vm.OrderHeader.PayementStatus = PaymentStatus.StatusPending;
            vm.OrderHeader.OrderDate = DateTime.Now;
            vm.OrderHeader.ApplicationUserId = claim.Value;
            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal = vm.ListOfCart.Sum(item => item.Product.Price * item.Count);
            }
            _unitOfWork.OrderHeader.Add(vm.OrderHeader);
            _unitOfWork.Save();
            foreach (var item in vm.ListOfCart)
            {
                OrderDetail detail = new OrderDetail()
                { 
                    ProductId = item.ProductId,
                    OrderHeaderId=vm.OrderHeader.Id,
                    Count = item.Count,
                    Price=item.Product.Price
                };
                _unitOfWork.OrderDetail.Add(detail);
                _unitOfWork.Save();
            }
            var domain = "https://localhost:7101/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
       
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/ordersuccess?id={vm.OrderHeader.Id}",
                CancelUrl = domain+$"customer/cart/Index",
            };
            foreach (var item in vm.ListOfCart)
            {

                var lineItemsOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price*100),
                        Currency = "PKR",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
             options.LineItems.Add(lineItemsOptions);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.PaymentStatus(vm.OrderHeader.Id, session.Id,session.PaymentIntentId);
            _unitOfWork.Save();
            _unitOfWork.Cart.DeleteRange(vm.ListOfCart);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ordersuccess(int id) 
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower()=="paid")
            {
             _unitOfWork.OrderHeader.UpdateStatus(id,OrderStatus.StatusApproved,PaymentStatus.StatusApproved);
            }
            List<Cart> cart= _unitOfWork.Cart.GetAll(x=>x.ApplicationUserId==orderHeader.ApplicationUserId).ToList();
            _unitOfWork.Cart.DeleteRange(cart);
            _unitOfWork.Save();
            return View(id); 
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
                var count = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count-1;
                HttpContext.Session.SetInt32("SessionCart", count);
                return RedirectToAction(nameof(Index));
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
            var count = _unitOfWork.Cart.GetAll(x=>x.ApplicationUserId==cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("SessionCart", count);
            return RedirectToAction(nameof(Index));
        }

        //public IEnumerable<Cart> ListOfCart { get; private set; }


       
    }
}
