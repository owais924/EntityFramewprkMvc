using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id, includeProperties: "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(x => x.Id == id, includeProperties: "Product");

            OrderView orderView = new OrderView
            {
                OrderHeader = orderHeader,
                OrderDetail = orderDetails // Assuming OrderDetails is a List or IEnumerable type
            };

            return View(orderView);
        }


    }
}
