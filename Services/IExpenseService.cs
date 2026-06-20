using ShareUp.API.DTOs;

namespace ShareUp.API.Services;

public interface IExpenseService
{
  Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);
  Task<IEnumerable<GroupDto>> GetAllGroupsAsync();
  Task<MemberDto> AddMemberAsync(Guid groupId, CreateMemberDto dto);
  Task<ExpenseDto> AddExpenseAsync(Guid groupId, CreateExpenseDto dto);
  Task<GroupDto?> GetGroupByIdAsync(Guid id);
}
