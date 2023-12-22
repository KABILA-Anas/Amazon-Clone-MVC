using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommerce.Models;

namespace ECommerce.Data
{
    public class ECommerceContext : DbContext
    {
        public ECommerceContext (DbContextOptions<ECommerceContext> options)
            : base(options)
        {
        }

        public DbSet<ECommerce.Models.Product> Product { get; set; } = default!;
        public DbSet<ECommerce.Models.Category> Category { get; set; } = default!;

    }
}
