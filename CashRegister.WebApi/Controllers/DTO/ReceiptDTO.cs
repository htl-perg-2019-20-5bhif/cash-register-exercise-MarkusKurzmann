using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashRegister.WebApi.Controllers.DTO
{
    public class ReceiptDTO
    {
        public List<Product> Products { get; set; }
        public float TotalAmount { get; set; }

    }
}
