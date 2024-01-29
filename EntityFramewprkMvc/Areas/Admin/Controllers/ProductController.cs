using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.DAL;
using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModel;

namespace EntityFrameworkMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitofwork;

        public ProductController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            ProductView productView = new ProductView();
            productView.Products = _unitofwork.Product.GetAll();
            return View(productView);
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitofwork.Category.Add(category);
        //        _unitofwork.Save();
        //        TempData["success"] = "Category Created Done!";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = _unitofwork.Product.GetT(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(CategoryView vm)
        {
            if (ModelState.IsValid)
            {
                if(vm.Category.Id == 0) 
                {
                _unitofwork.Category.Add(vm.Category);
                TempData["success"] = "Category Created Done!";
                }
                else
                {
                    _unitofwork.Category.Update(vm.Category);
                    TempData["success"] = "Category Updated Done!";
                }
                _unitofwork.Save();
               // TempData["success"] = "Category Updated Done!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _unitofwork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View();
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var category = _unitofwork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitofwork.Category.Delete(category);
            _unitofwork.Save();
            TempData["success"] = "Category Deleted Done!";
            return RedirectToAction("Index");



        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id) 
        {
            ProductView vm = new ProductView();
            {
                //Product = new(),
                // Categories = _unitofwork.Category.GetAll().Select(x =>
                // new SelectListItem()
                // {
                //   Text = x.Name,
                //   Value=x.Id.ToString()
                // })
                 
            };
            if(id == null || id == 0) { return View(vm); }
            else
            {
                vm.Product=_unitofwork.Product.GetT(x=>x.Id == id);
                if(vm.Product == null) 
                {
                    return NotFound();   
                }
                else 
                {
                    return View(vm);
                }
            }
        }
    }
}
