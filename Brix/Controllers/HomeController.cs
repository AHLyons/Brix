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

namespace Brix.Controllers
{
    public class HomeController : Controller
    {
        private ILegostoreRepository _repo;
        private readonly InferenceSession _session;
        private readonly string _onnxModelPath;

        public HomeController(ILegostoreRepository temp, InferenceSession session, IHostEnvironment hostEnvironment)
        {
            _repo = temp;
            _session = session;

            _onnxModelPath = System.IO.Path.Combine(hostEnvironment.ContentRootPath, "wwwroot", "decision_tree_model-3.onnx");
            System.IO.Path.Combine(hostEnvironment.ContentRootPath, "wwwroot", "decision_tree_model-3.onnx");

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

        ////[Authorize]
        //[HttpGet]
        //[Authorize] // Ensure the user is logged in
        //public async Task<IActionResult> Manage()
        //{
        //    var userEmail = User.Identity.Name; // Ensure this is how you identify users
        //    var customer = await _repo.GetCustomerByEmailAsync(userEmail);

        //    if (customer == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    return View(customer);
        //}


        //[HttpPost]
        //[Authorize] // Ensure the user is logged in
        //public async Task<IActionResult> EditUsers(Customer customer)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Manage", customer);
        //    }

        //    var userEmail = User.Identity.Name; // Or your method of identifying the user
        //    var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmail);

        //    if (existingCustomer == null || existingCustomer.CustomerId != customer.CustomerId)
        //    {
        //        return Unauthorized(); // Prevents users from editing other users' data
        //    }

        //    // Update the properties you allow to be changed
        //    existingCustomer.FirstName = customer.FirstName;
        //    existingCustomer.LastName = customer.LastName;
        //    // and so on for other editable fields...

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Manage"); // Or wherever you want to redirect after the update
        //}


        public IActionResult AEDUser()
        {
            var customers = _repo.Customers.ToList(); // Assuming _repo.Customers is an IQueryable<Customer>
            return View(customers);
        }


        public IActionResult About()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }

        //public IActionResult FraudCheck()
        //{
        //    return View();
        //}
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
                _repo.UpdateProduct(product); // Assuming this method updates the product in your repository
                return RedirectToAction("AEDProduct"); // Redirect to the AEDProduct view after successful edit
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

        // Action for displaying form to add a new customer
        [HttpGet]
        public IActionResult NewUser()
        {
            return View(new Customer());
        }

        // Action to handle new customer form submission
        [HttpPost]
        public async Task<IActionResult> NewUser(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _repo.NewCustomer(customer);
                return RedirectToAction("AEDUser"); // Make sure to redirect to the correct action that shows the list of customers
            }
            return View(customer);
        }

        // Action for displaying customer edit form
        public IActionResult EditUsers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _repo.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // Action to handle customer edit form submission
        [HttpPost]
        public IActionResult EditUsers(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repo.UpdateCustomer(customer);
                return RedirectToAction("AEDUser"); // Adjust as needed
            }
            return View(customer);
        }

        // Action for displaying customer delete confirmation
        public IActionResult DeleteUsers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _repo.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // Action to confirm customer deletion
        // Action to confirm customer deletion
        [HttpPost, ActionName("DeleteUsers")]
        public IActionResult DeleteCustomerConfirmed(int id)
        {
            var customer = _repo.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer != null)
            {
                _repo.DeleteCustomer(customer);
            }
            // Redirect to the AEDUser action to show the updated list of users
            return RedirectToAction("AEDUser");
        }




    }
}
