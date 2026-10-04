using System.Text.Json.Serialization;

namespace InventoryManagementAPI.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();

        [JsonIgnore]
        public ICollection<Operation> Operations { get; set; } = new List<Operation>();
    }
}