using System.Text.Json.Serialization; // <--- 

namespace InventoryManagementAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [JsonIgnore] // <--- تمنع إظهار المنتجات داخل نموذج إضافة الفئة
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}