using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.CommonHelper;
using MyApp.DAL;
using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModel;

namespace EntityFrameworkMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =WebsiteRole.Role_Admin)]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitofwork;

        public CategoryController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            CategoryView categoryView = new CategoryView();
            categoryView.categories = _unitofwork.Category.GetAll();
            return View(categoryView);
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
            var category = _unitofwork.Category.GetT(x => x.Id == id);
            if (category == null)
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
            CategoryView vm = new CategoryView();
            if(id == null || id == 0) { return View(vm); }
            else
            {
                vm.Category=_unitofwork.Category.GetT(x=>x.Id == id);
                if(vm.Category == null) 
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
