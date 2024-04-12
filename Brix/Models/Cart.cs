namespace Brix.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public virtual void AddItem(Product prod, int quantity)
        {
            CartLine? Line = Lines
                .Where(p => p.Product.ProductId == prod.ProductId)
                .FirstOrDefault();

            if (Line == null)
            {
                Lines.Add(new CartLine
                {
                    Product = prod,
                    Quantity = quantity
                });
            }
            else
            {
                Line.Quantity += quantity;
            }
        }

        public virtual void RemoveLine(Product prod) => Lines.RemoveAll(l => l.Product.ProductId == prod.ProductId);

        public virtual void Clear() => Lines.Clear();

        public decimal CalculateTotal() => (int)Lines.Sum(e => e.Product.Price * e.Quantity);

        public class CartLine
        {
            public int CartLineId { get; set; }
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }   
    }
}
