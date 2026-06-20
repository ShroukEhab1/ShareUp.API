using System;

namespace ShareUp.API.DTOs;

public class CreateGroupDto
{
  public string Name { get; set; } = string.Empty;
}

public class GroupDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public List<MemberDto> Members { get; set; } = new List<MemberDto>();
  public List<ExpenseDto> Expenses { get; set; } = new List<ExpenseDto>();
}
