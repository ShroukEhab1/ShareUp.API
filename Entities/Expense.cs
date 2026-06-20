namespace ShareUp.API.Entities
{
  public class Expense
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid GroupId { get; set; }
    public decimal Amount { get; set; }
    public Guid PaidById { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ExpenseShare> ExpenseShares { get; set; } = new List<ExpenseShare>();
  }
}
