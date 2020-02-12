using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CashRegister.WebApi.Controllers
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [Required]
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [Required]
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }

    public class ReceiptLine
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("product")]
        public Product Product { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [Required]
        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    }

    public class Receipt
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [Required]
        [JsonPropertyName("receiptTimestamp")]
        public DateTime ReceiptTimestamp { get; set; }

        [JsonPropertyName("receiptLines")]
        public List<ReceiptLine> ReceiptLines { get; set; }

        [Required]
        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
