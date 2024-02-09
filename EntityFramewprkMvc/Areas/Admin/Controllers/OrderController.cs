using EntityFrameworkMvc.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyApp.CommonHelper;
using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModel;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace EntityFrameworkMvc.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private  IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            
            _unitOfWork= unitOfWork;
        }
        #region APICALL
        public IActionResult AllOrders(string status)
        {
            IEnumerable<OrderHeader> orderHeader;
           
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                orderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                orderHeader = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == claim.Value);
            }
            switch (status)
            {
                case "pending":
                    orderHeader = orderHeader.Where(x => x.PayementStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    orderHeader = orderHeader.Where(x => x.PayementStatus == PaymentStatus.StatusApproved);
                    break;
                case "underprocess":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusInProcess);
                    break;
                case "shipped":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusShipped);
                    break;
              
                default:
                    break;
            }
          
           
            return Json(new {data = orderHeader });
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OrderDetails(int id)
        {
            //var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id, includeProperties: "ApplicationUser");
            //var orderDetails = _unitOfWork.OrderDetail.GetAll(x => x.Id == id, includeProperties: "Product");

            OrderView orderView = new OrderView()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.Id == id, includeProperties: "Product")
            };
            //OrderHeader = orderHeader,
            //    OrderDetail = orderDetails // Assuming OrderDetails is a List or IEnumerable type


            return View(orderView);
        }
        
        [Authorize(Roles =WebsiteRole.Role_Admin+","+WebsiteRole.Role_Employee)]
        [HttpPost]
        public IActionResult OrderDetails(OrderView orderview )
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == orderview.OrderHeader.Id);
            orderHeader.Name= orderview.OrderHeader.Name;
            orderHeader.Phone= orderview.OrderHeader.Phone;
            orderHeader.Address = orderview.OrderHeader.Address;
            orderHeader.City= orderview.OrderHeader.City;
            orderHeader.State= orderview.OrderHeader.State;
            orderHeader.PostalCode= orderview.OrderHeader.PostalCode;
            if(orderview.OrderHeader.Carrier!=null)
            {
                orderHeader.Carrier= orderview.OrderHeader.Carrier;
            }
            if (orderview.OrderHeader.TrackingNumber != null)
            {
                orderHeader.TrackingNumber = orderview.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Info Updated";
            return RedirectToAction("OrderDetails","Order", new {id=orderview.OrderHeader.Id});
           
        }
        [Authorize(Roles = WebsiteRole.Role_Admin + "," + WebsiteRole.Role_Employee)]
        public IActionResult InProcess(OrderView orderview)
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderview.OrderHeader.Id,OrderStatus.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated-InProcess";
            return RedirectToAction("OrderDetails", "Order", new { id = orderview.OrderHeader.Id });

        }
        [Authorize(Roles = WebsiteRole.Role_Admin + "," + WebsiteRole.Role_Employee)]
        public IActionResult Shipped(OrderView orderview)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == orderview.OrderHeader.Id);
            orderHeader.Carrier = orderview.OrderHeader.Carrier;
            orderHeader.TrackingNumber = orderview.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = OrderStatus.StatusShipped;
            orderHeader.ShippingDate=DateTime.Now;
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated-InShipped";
            return RedirectToAction("OrderDetails", "Order", new { id = orderview.OrderHeader.Id });
        }
        [Authorize(Roles = WebsiteRole.Role_Admin + "," + WebsiteRole.Role_Employee)]
        public IActionResult CancelOrder(OrderView orderview)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == orderview.OrderHeader.Id);
         

            if (orderHeader.PayementStatus==PaymentStatus.StatusApproved)
            {
                var refund = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund resultRefund = service.Create(refund);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, OrderStatus.StatusCancelled, OrderStatus.StatusRefund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, OrderStatus.StatusCancelled, OrderStatus.StatusCancelled);
            }
           
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled";
            return RedirectToAction("OrderDetails", "Order", new { id = orderview.OrderHeader.Id });
        }
        public IActionResult PayNow(OrderView orderview)
        {
            var OrderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == orderview.OrderHeader.Id, includeProperties: "ApplicationUser");
            var OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderview.OrderHeader.Id, includeProperties: "Product");

            var domain = "https://localhost:7101/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/ordersuccess?id={orderview.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/Index",
            };
            foreach (var item in OrderDetail)
            {

                var lineItemsOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
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
            _unitOfWork.OrderHeader.PaymentStatus(orderview.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            return RedirectToAction("Index", "Home");
        }

    }
}
