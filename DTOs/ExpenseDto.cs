using System;
using System.Collections.Generic;

namespace ShareUp.API.DTOs;

public class CreateExpenseDto
{
  public string Title { get; set; } = string.Empty;
  public decimal Amount { get; set; } 
  public Guid PaidById { get; set; } 

  public List<Guid> ParticipantIds { get; set; } = new List<Guid>();
}

public class ExpenseDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public Guid PaidById { get; set; }
  public DateTime CreatedAt { get; set; }
  public List<Guid> ParticipantIds { get; set; } = new List<Guid>();
}
