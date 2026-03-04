using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace EducatITion.DB.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public decimal TotalPrice
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                {
                    if (!string.IsNullOrEmpty(item.Price))
                    {
                        var priceRange = item.Price.Replace("$", "").Split('-');
                        if (priceRange.Length > 0 && decimal.TryParse(priceRange[0], out decimal minPrice))
                        {
                            total += minPrice * item.Quantity;
                        }
                    }
                }
                return total;
            }
        }

        public int TotalItems => Items.Sum(i => i.Quantity);

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.CourseId == item.CourseId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                Items.Add(item);
            }
            UpdatedAt = DateTime.Now;
        }

        public void RemoveItem(int courseId)
        {
            var item = Items.FirstOrDefault(i => i.CourseId == courseId);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    Items.Remove(item);
                }
                UpdatedAt = DateTime.Now;
            }
        }

        public void RemoveItemCompletely(int courseId)
        {
            Items.RemoveAll(i => i.CourseId == courseId);
            UpdatedAt = DateTime.Now;
        }

        public void Clear()
        {
            Items.Clear();
            UpdatedAt = DateTime.Now;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Cart FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new Cart();

            try
            {
                return JsonSerializer.Deserialize<Cart>(json);
            }
            catch
            {
                return new Cart();
            }
        }
    }
}
