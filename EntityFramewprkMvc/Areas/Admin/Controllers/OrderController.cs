using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyApp.CommonHelper;
using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModel;
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
        public IActionResult InProcess(OrderView orderview)
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderview.OrderHeader.Id,OrderStatus.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated-InProcess";
            return RedirectToAction("OrderDetails", "Order", new { id = orderview.OrderHeader.Id });

        }
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



    }
}
