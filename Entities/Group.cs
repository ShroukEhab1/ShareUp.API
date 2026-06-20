namespace ShareUp.API.Entities
{
  public class Group
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Member> Members { get; set; } = new List<Member>();
  }
}
