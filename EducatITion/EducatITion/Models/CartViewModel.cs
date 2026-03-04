using EducatITion.DB.Models;

namespace EducatITion.Models
{
    public class CartViewModel : BaseViewModel
    {
        public Cart Cart { get; set; }
        public List<CartItem> Items => Cart?.Items ?? new List<CartItem>();
        public int TotalItems => Cart?.TotalItems ?? 0;
        public decimal TotalPrice => Cart?.TotalPrice ?? 0;
        public bool IsEmpty => Cart == null || !Cart.Items.Any();

        public string EmptyCartMessage { get; set; }
        public string CheckoutButtonText { get; set; }
        public string ContinueShoppingText { get; set; }
        public string RemoveButtonText { get; set; }

        public CartViewModel(ISession session) : base(session) { }

        protected override void LoadData(Localization localization)
        {
            var cartJson = Session.GetString("Cart");
            Cart = Cart.FromJson(cartJson);
        }

        protected override void Localize(Localization localization)
        {
            if (localization == Localization.ru)
            {
                Title = "Корзина";
                EmptyCartMessage = "Ваша корзина пуста";
                CheckoutButtonText = "Оформить заказ";
                ContinueShoppingText = "Продолжить покупки";
                RemoveButtonText = "Удалить";
            }
            else
            {
                Title = "Shopping Cart";
                EmptyCartMessage = "Your cart is empty";
                CheckoutButtonText = "Checkout";
                ContinueShoppingText = "Continue Shopping";
                RemoveButtonText = "Remove";
            }
        }
    }
}
