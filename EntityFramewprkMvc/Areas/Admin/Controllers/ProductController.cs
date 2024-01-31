using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using MyApp.DAL.Infrastructure.IRepository;
using MyApp.Models.ViewModel;

namespace EntityFrameworkMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitofwork;
        private IWebHostEnvironment _hostingEnvironment;//ye image phoncha ga www.root folder ma

       

        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment hostingEnvironment)
        {
            _unitofwork = unitofwork;
            _hostingEnvironment = hostingEnvironment;
        }
        #region APICALLING
        public IActionResult AllProducts()
        {
            var products = _unitofwork.Product.GetAll(includeProperties:"Category");
            return Json(new  {data=products});
        }
        #endregion

        public IActionResult Index()
        {
            //ProductView productView = new ProductView();
            //productView.Products = _unitofwork.Product.GetAll();
            return View();
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
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductView vm = new ProductView();
            {
                vm.Product = new();
                vm.Categories = _unitofwork.Category.GetAll().Select(x =>
                    new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });
            };

                if (id == null || id == 0) { return View(vm); }
                else
                {
                    vm.Product = _unitofwork.Product.GetT(x => x.Id == id);
                    if (vm.Product == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return View(vm);
                    }
                }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductView vm, IFormFile? file)
        {
            if (ModelState.IsValid)
            { 
                string fileName=null;
                if (file != null) 
                {
                    string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImage");//uploadDir ma pora path a gaya
                    fileName=Guid.NewGuid().ToString()+"-"+file.FileName;//file name kabhi b duplicate nahi hona chya
                    string filePath=Path.Combine(uploadDir, fileName);
                    if(vm.Product.ImageUrl != null)
                    {
                        var oldImagePath=Path.Combine(_hostingEnvironment.WebRootPath,vm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                           System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream=new FileStream(filePath,FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    vm.Product.ImageUrl = @"\ProductImage\" + fileName;
                }
                if(vm.Product.Id == 0)
                {
                    _unitofwork.Product.Add(vm.Product);
                    TempData["success"] = "Product Created Done!";
                }
                else
                {
                    _unitofwork.Product.Update(vm.Product);
                    TempData["success"] = "Product Updated Done!";
                }




                _unitofwork.Save();
               // TempData["success"] = "Category Updated Done!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }
        #region APIDELETECALL
        [HttpDelete]
        
        public IActionResult Delete(int? id)
        {
            var product = _unitofwork.Product.GetT(x => x.Id == id);
            if (product == null)
            {
                return Json(new {success = false, message="Error occured"});
            }
            else
            {
                var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitofwork.Product.Delete(product);
                 _unitofwork.Save();
                return Json(new { success = true, message = "Product Deleted" });
            }

        }
        #endregion

    }
}
