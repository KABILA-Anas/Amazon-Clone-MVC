namespace ECommerce.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public List<CartItem>? Items { get; set; } = new List<CartItem>();

        public double SubTotal()
        {
            float total = 0;
            foreach (var item in Items)
            {
                total += item.Product.Price * item.Quantity.GetValueOrDefault();
            }

            return total;
        }

        public double TotalPrice()
        {
            float total = 0;
            foreach (var item in Items)
            {
                total += item.Product.Price * item.Quantity.GetValueOrDefault();
            }

            return total + 4.99;
        }

        public int Size()
        {
            /*int size = 0;
            foreach (var item in Items)
            {
                size += item.Quantity.GetValueOrDefault();
            }

            return size;*/

            return Items.Sum(item => item.Quantity.GetValueOrDefault());

        }

        public void AddItem(Product product, int quantity=1)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Product = product
                });
            }
        }
    }
}
