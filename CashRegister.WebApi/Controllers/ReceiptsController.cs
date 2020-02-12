using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashRegister.WebApi.Controllers.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashRegister.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly DataContext CashContext;

        public ReceiptsController(DataContext dataContext)
        {
            CashContext = dataContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<ReceiptLineDto> receiptLineDto)
        {
            if (receiptLineDto == null || receiptLineDto.Count == 0)
            {
                return BadRequest("There has to be at least one receipt line");
            }

            // Read product data from DB for incoming product IDs
            var products = new Dictionary<int, Product>();
            foreach (var rl in receiptLineDto)
            {
                products[rl.ProductID] = await CashContext.Product.FirstOrDefaultAsync(p => p.ID == rl.ProductID);
                if (products[rl.ProductID] == null)
                {
                    return BadRequest($"Unknown product ID {rl.ProductID}");
                }
            }

            // Build receipt from DTO
            var newReceipt = new Receipt
            {
                ReceiptTimestamp = DateTime.UtcNow,
                ReceiptLines = receiptLineDto.Select(rl => new ReceiptLine
                {
                    ID = 0,
                    Product = products[rl.ProductID],
                    Amount = rl.Amount,
                    TotalPrice = rl.Amount * products[rl.ProductID].UnitPrice
                }).ToList()
            };
            newReceipt.TotalPrice = newReceipt.ReceiptLines.Sum(rl => rl.TotalPrice);

            await CashContext.Receipt.AddAsync(newReceipt);
            await CashContext.SaveChangesAsync();

            return Created("Receipt has been created", newReceipt);
        }
    }
}
