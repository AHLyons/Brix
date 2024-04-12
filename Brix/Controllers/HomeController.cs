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
        private ILegoStoreRepository _repo;

        public HomeController(ILegoStoreRepository temp)
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


        public IActionResult Products(int pageNum)
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

        public IActionResult OrderConfirmationGood()
        {
            return View();
        }
        public IActionResult OrderConfirmationBad()
        {
            return View();
        }
    }
}
