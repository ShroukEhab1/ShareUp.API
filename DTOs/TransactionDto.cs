using System;

namespace ShareUp.API.DTOs;

public class TransactionDto
{
  public Guid FromMemberId { get; set; }
  public string FromMemberName { get; set; } = string.Empty;

  public Guid ToMemberId { get; set; }
  public string ToMemberName { get; set; } = string.Empty;

  public decimal Amount { get; set; }
}
