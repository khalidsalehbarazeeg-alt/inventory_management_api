using System.Text.Json.Serialization;

namespace InventoryManagementAPI.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public string OperationType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        public int? SupplierId { get; set; }
        [JsonIgnore]
        public Supplier? Supplier { get; set; }
    }
}