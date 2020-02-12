using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CashRegister.WebApi.Controllers;

namespace CashRegister.WebApi.Controllers
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<CashRegister.WebApi.Controllers.Product> Product { get; set; }

        public DbSet<CashRegister.WebApi.Controllers.Receipt> Receipt { get; set; }

        public DbSet<CashRegister.WebApi.Controllers.ReceiptLine> ReceiptLine { get; set; }
    }
}
