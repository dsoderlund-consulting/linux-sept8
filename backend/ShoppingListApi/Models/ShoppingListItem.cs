namespace ShoppingListApi.Models
{
    public class ShoppingListItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty; // Initialize to avoid null warnings
        public bool IsDone { get; set; }
    }
}