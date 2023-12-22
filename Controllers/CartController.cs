using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "Administrator, Customer")]
    public class CartController : Controller
    {

        private readonly ECommerceContext _context;
        private readonly ILogger _logger;

        public CartController(ECommerceContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            //get cart from session or create new cart using serialization
            var item = HttpContext.Session.GetString("Cart");
            Cart cart = item == null ? new Cart() : JsonSerializer.Deserialize<Cart>(item);

            //store items in view bag
            ViewBag.Cart = cart;

            //store total price in view bag
            ViewBag.TotalPrice = cart.TotalPrice();


            //return view
            return View();
        }

        public IActionResult AddToCart(int productId)
        {
            _logger.LogInformation("Starting the AddToCart request.");

            //get cart from session or create new cart using serialization
            var item = HttpContext.Session.GetString("Cart");
            Cart cart = item == null ? new Cart() : JsonSerializer.Deserialize<Cart>(item);

            _logger.LogInformation("Cart retrieved from session.");

            // get product from database
            var product = _context.Product.Find(productId);

            //add product to cart
            cart.AddItem(product);

            _logger.LogInformation("Product added to cart.");

            //save cart to session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

            _logger.LogInformation("AddToCart request completed successfully.");

            //redirect to index
            return RedirectToAction("Index", "Cart");
        }

        //get size of cart
        public IActionResult GetCartSize()
        {
            //get cart from session or create new cart using serialization
            var item = HttpContext.Session.GetString("Cart");
            Cart cart = item == null ? new Cart() : JsonSerializer.Deserialize<Cart>(item);
            //return size of cart in view bag
            ViewBag.Size = cart.Size();
            return View();
        }
    }
}
