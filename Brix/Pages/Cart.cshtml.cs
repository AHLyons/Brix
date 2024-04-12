using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Brix.Models;
using Brix.Infrastructure;
using System.Text.Json.Serialization;

namespace Brix.Pages
{
    public class CartModel : PageModel
    {
        private ILegoStoreRepository _repo;
        public Cart cart { get; set; }

        public CartModel(ILegoStoreRepository temp, Cart cartService)
        {
            _repo = temp;
            cart = cartService;
        }   

        public string ReturnUrl { get; set; } = "/";

        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            //cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        }

        public IActionResult OnPost(int ProductId, string returnUrl) 
        {
            Product prod = _repo.Products
                .FirstOrDefault(x => x.ProductId == ProductId);

            if (prod != null)
            {
                //cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(prod, 1);
                //HttpContext.Session.SetJson("cart", cart);
            }

            return RedirectToPage(new { returnUrl = returnUrl });
        }

    }
}
