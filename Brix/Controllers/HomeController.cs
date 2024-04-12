using Brix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Brix.Models.ViewModels;
using SQLitePCL;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.EntityFrameworkCore;
using Elfie.Serialization;
using Microsoft.AspNetCore.Hosting;
using Brix.Infrastructure;

namespace Brix.Controllers
{
    public class HomeController : Controller
    {
        private ILegoStoreRepository _repo;
        private readonly InferenceSession _session;
        private readonly string _onnxModelPath;

        public HomeController(ILegoStoreRepository temp, InferenceSession session, IHostEnvironment hostEnvironment)
        {
            _repo = temp;
            _session = session;

            _onnxModelPath = System.IO.Path.Combine(hostEnvironment.ContentRootPath, "decision_tree_model-3.onnx");
            System.IO.Path.Combine(hostEnvironment.ContentRootPath, "decision_tree_model-3.onnx");

            _session = new InferenceSession(_onnxModelPath);



            try
            {
                _session = new InferenceSession("./decision_tree_model-3.onnx");
                //_logger.LogInformation("ONNX model loaded successfully.");
                Console.WriteLine("ONNX model loaded successfully.");
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error loading the ONNX model: {ex.Message}");
                Console.WriteLine($"Error loading the ONNX model: {ex.Message}");
            }
        }

        //public IActionResult Index(int pageNum)
        //{
        //    int pageSize = 10;
        //    if (pageNum < 1)
        //    {
        //        pageNum = 1;
        //    }

        //    var blah = new LegosListViewModel
        //    {
        //        Products = _repo.Products
        //            .OrderBy(x => x.ProductId)
        //            .Skip((pageNum - 1) * pageSize)
        //            .Take(pageSize),

        //        PaginationInfo = new PaginationInfo
        //        {
        //            CurrentPage = pageNum,
        //            ItemsPerPage = pageSize,
        //            TotalItems = _repo.Products.Count()
        //        }

        //    };

        //    return View(blah);
        //}

        public IActionResult FraudCheck()
        {
            var records = _repo.Orders
                .OrderByDescending(x => x.Date)
                .Take(20)
                .ToList();  // Fetch all records
            var predictions = new List<FraudPrediction>();  // Your ViewModel for the view

            // Dictionary mapping the numeric prediction to an animal type
            var fraud_dict = new Dictionary<int, string>
           {
               { 0, "Not Fraud" },
               { 1, "Fraud" }
           };

            foreach (var record in records)
            {
                var input = new List<float>
                {
                    (float)record.Date,
                    (float)record.Time,
                    (float)record.Amount,
                    1,
                    record.DayOfWeek == "Mon" ? 1 : 0,
                    record.DayOfWeek == "Sat" ? 1 : 0,
                    record.DayOfWeek == "Sun" ? 1 : 0,
                    record.DayOfWeek == "Thu" ? 1 : 0,
                    record.DayOfWeek == "Tue" ? 1 : 0,
                    record.DayOfWeek == "Wed" ? 1 : 0,
                    record.EntryMode == "PIN" ? 1 : 0,
                    record.EntryMode == "Tap" ? 1 : 0,
                    record.TypeOfTransaction == "Online" ? 1 : 0,
                    record.TypeOfTransaction == "POS" ? 1 : 0,
                    record.CountryOfTransaction == "India" ? 1 : 0,
                    record.CountryOfTransaction == "Russia" ? 1 : 0,
                    record.CountryOfTransaction == "USA" ? 1 : 0,
                    record.CountryOfTransaction == "United Kingdom" ? 1 : 0,
                    record.ShippingAddress == "India" ? 1 : 0,
                    record.ShippingAddress == "Russia" ? 1 : 0,
                    record.ShippingAddress == "USA" ? 1 : 0,
                    record.ShippingAddress == "United Kingdom" ? 1 : 0,
                    record.Bank == "HSBC" ? 1 : 0,
                    record.Bank == "Halifax" ? 1 : 0,
                    record.Bank == "Lloyds" ? 1 : 0,
                    record.Bank == "Metro" ? 1 : 0,
                    record.Bank == "Monzo" ? 1 : 0,
                    record.Bank == "RBS" ? 1 : 0,
                    record.TypeOfCard == "Visa" ? 1 : 0
                };
                var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

                var inputs = new List<NamedOnnxValue>
               {
                   NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
               };

                string predictionResult;
                using (var results = _session.Run(inputs))
                {
                    var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                    predictionResult = prediction != null && prediction.Length > 0 ? fraud_dict.GetValueOrDefault((int)prediction[0], "Unknown") : "Error in prediction";
                }

                predictions.Add(new FraudPrediction { Orders = record, Prediction = predictionResult }); // Adds the fraud information and prediction for that fraud to FraudPrediction viewmodel
            }

            return View(predictions);
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


        // In HomeController.cs
        // In HomeController.cs
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

        public SessionCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()
                .HttpContext?.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

    }
}
