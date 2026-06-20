namespace ShareUp.API.Entities
{
  public class Member
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid GroupId { get; set; }
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<ExpenseShare> ExpenseShares { get; set; } = new List<ExpenseShare>();
  }
}
