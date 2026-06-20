using System;

namespace ShareUp.API.DTOs;

public class CreateMemberDto
{
  public string Name { get; set; } = string.Empty;
}

public class MemberDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Guid GroupId { get; set; }
}
