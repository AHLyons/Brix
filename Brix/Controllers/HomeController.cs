using Brix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Brix.Models.ViewModels;
using SQLitePCL;

namespace Brix.Controllers
{
    public class HomeController : Controller
    {
        private ILegostoreRepository _repo;

        public HomeController(ILegostoreRepository temp)
        {
            _repo = temp;
        }

        public IActionResult Index(int pageNum)
        {
            int pageSize = 10;
            if (pageNum < 1)
            {
                pageNum = 1;
            }

            var blah = new LegosListViewModel
            {
                Products = _repo.Products
                    .OrderBy(x => x.ProductId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _repo.Products.Count()
                }

            };

            return View(blah);
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

        //[Authorize]
        public IActionResult Manage()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }

        //public IActionResult AEDProduct()
        //{
        //    var products = _repo.Products.ToList(); // Assuming _repo.Products is an IQueryable<Product>
        //    return View(products);
        //}

        [HttpGet]
        public IActionResult NewProduct()
        {
            return View(new Product()); // Returns an empty form
        }

        [HttpPost]
        public async Task<IActionResult> NewProduct(Product product) // Mark this method as async
        {
            if (ModelState.IsValid)
            {
                await _repo.NewProduct(product); // Await the async NewProduct method

                return RedirectToAction("AEDProduct"); // Redirect to the AEDProduct view
            }
            return View(product);
        }

        public IActionResult AEDProduct()
        {
            var products = _repo.Products.ToList();
            return View(products);
        }



        public IActionResult FraudCheck()
        {
            return View();
        }

        public IActionResult ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _repo.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Products(int pageNum, string category, string color, int pageSize = 10)
        {
            if (pageNum < 1)
            {
                pageNum = 1;
            }

            var categories = _repo.Products.Select(p => p.Category).Distinct().ToList();
            var primaryColors = _repo.Products.Select(p => p.PrimaryColor).Distinct().ToList();

            var productsQuery = _repo.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category == category);
            }

            if (!string.IsNullOrEmpty(color))
            {
                productsQuery = productsQuery.Where(p => p.PrimaryColor == color);
            }

            var viewModel = new LegosListViewModel
            {
                Products = productsQuery
                    .OrderBy(x => x.ProductId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = productsQuery.Count()
                },
                CurrentCategory = category,
                CurrentColor = color,
                PageSize = pageSize,
                Categories = categories,
                PrimaryColors = primaryColors
            };

            return View(viewModel);
        }


        //[HttpPost]
        //public IActionResult NewProduct(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _repo.NewProduct(product);
        //        return RedirectToAction("Index");
        //    }
        //    return View("Products", _repo.Products);
        //}



        public IActionResult OrderConfirmation()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult NewProduct(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _repo.NewProduct(product);
        //        return RedirectToAction("Index");
        //    }
        //    return View(product);
        //}

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _repo.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repo.UpdateProduct(product); // Ensure this method is implemented in the repository
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _repo.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _repo.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                _repo.DeleteProduct(product); // Ensure this method is implemented in the repository
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
