using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "Administrator, Customer")]
    public class ProductsController : Controller
    {
        private readonly ECommerceContext _context;
        private readonly ILogger _logger;

        public ProductsController(ECommerceContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, int? productCategory)
        {
            /*var item = HttpContext.Session.GetString("Cart");
            Cart cart = item == null ? new Cart() : JsonSerializer.Deserialize<Cart>(item);
            //return size of cart in view bag
            ViewBag.CartSize = cart.Size();*/

            _logger.LogInformation("Starting the GET Products request.");

            var categories = await _context.Category.ToListAsync();

            IQueryable<Product> products = from m in _context.Product.Include(p => p.Category)
                                           select m;


            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            if (productCategory.HasValue)
            {
                products = products.Where(p => p.CategoryId == productCategory);
            }

            ViewBag.Categories = categories;
            var Products = await products.ToListAsync();

            _logger.LogInformation("GET Products request completed successfully.");

            return View(Products);
            
            /*var eCommerceContext = _context.Product.Include(p => p.Category);
            return View(await eCommerceContext.ToListAsync());*/
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImageContent,CategoryId")] Product product)
        {
            if (product.ImageContent != null)
            {
                var fileName = Path.GetFileName(product.ImageContent.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageContent.CopyToAsync(stream);
                }
                product.Image = fileName;
            }

            //search and affect category
            var category = _context.Category.FirstOrDefault(c => c.Id == product.CategoryId);
            if (category != null)
            {
                product.Category = category;
            }

            //update model state
            ModelState.Clear();
            TryValidateModel(product);

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Id", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Image,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Id", product.CategoryId);
            return View(product);
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
