namespace ShareUp.API.Entities
{
  public class ExpenseShare
  {
    public Guid Id { get; set; }
    public Guid ExpenseId { get; set; }
    public Guid MemberId { get; set; }
    public decimal ShareAmount { get; set; }

    public Expense Expense { get; set; } = null!;
    public Member Member { get; set; } = null!;
  }
}
