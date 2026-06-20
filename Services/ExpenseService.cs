using Microsoft.EntityFrameworkCore;
using ShareUp.API.Data;
using ShareUp.API.DTOs;
using ShareUp.API.Entities;

namespace ShareUp.API.Services;

public class ExpenseService : IExpenseService
{
  private readonly ShareUpDbContext _context;

  public ExpenseService(ShareUpDbContext context)
  {
    _context = context;
  }


  public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto)
  {
    var group = new Group { Name = dto.Name };
    _context.Groups.Add(group);
    await _context.SaveChangesAsync();

    return new GroupDto { Id = group.Id, Name = group.Name, CreatedAt = group.CreatedAt };
  }

  public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync()
  {
    var groups = await _context.Groups
        .Include(g => g.Members)
        .Include(g => g.Expenses)
            .ThenInclude(e => e.ExpenseShares) // 👈 عمل Include لجدول الربط
        .ToListAsync();

    return groups.Select(group => new GroupDto
    {
      Id = group.Id,
      Name = group.Name,
      CreatedAt = group.CreatedAt,
      Members = group.Members.Select(m => new MemberDto
      {
        Id = m.Id,
        Name = m.Name
      }).ToList(),
      Expenses = group.Expenses.Select(e => new ExpenseDto
      {
        Id = e.Id,
        Title = e.Title,
        Amount = e.Amount,
        PaidById = e.PaidById,
        // 🔥 ملء قائمة المشاركين للأنجولر
        ParticipantIds = e.ExpenseShares.Select(es => es.MemberId).ToList()
      }).ToList()
    }).ToList();
  }
  public async Task<MemberDto> AddMemberAsync(Guid groupId, CreateMemberDto dto)
  {
    var member = new Member { Name = dto.Name, GroupId = groupId };
    _context.Members.Add(member);
    await _context.SaveChangesAsync();

    return new MemberDto { Id = member.Id, Name = member.Name, GroupId = member.GroupId };
  }

  public async Task<ExpenseDto> AddExpenseAsync(Guid groupId, CreateExpenseDto dto)
  {
    if (dto.ParticipantIds == null || dto.ParticipantIds.Count == 0)
    {
      throw new ArgumentException("يجب اختيار عضو واحد على الأقل للمشاركة في المصروف");
    }

    var expense = new Expense
    {
      Id = Guid.NewGuid(),
      GroupId = groupId,
      Title = dto.Title,
      Amount = dto.Amount,
      PaidById = dto.PaidById,
      CreatedAt = DateTime.UtcNow
    };

    _context.Expenses.Add(expense);

    decimal shareAmount = dto.Amount / dto.ParticipantIds.Count;

    foreach (var participantId in dto.ParticipantIds)
    {
      var share = new ExpenseShare
      {
        Id = Guid.NewGuid(),
        ExpenseId = expense.Id,
        MemberId = participantId,
        ShareAmount = shareAmount
      };
      _context.ExpenseShares.Add(share);
    }

    await _context.SaveChangesAsync();

    return new ExpenseDto
    {
      Id = expense.Id,
      Title = expense.Title,
      Amount = expense.Amount,
      PaidById = expense.PaidById,
      ParticipantIds = dto.ParticipantIds // 🔥 نرجعها فوراً عشان تضاف في الفرونت إند بدون ثواني تأخير
    };
  }
  public async Task<GroupDto?> GetGroupByIdAsync(Guid id)
  {
    var group = await _context.Groups
        .Include(g => g.Members)
        .Include(g => g.Expenses)
            .ThenInclude(e => e.ExpenseShares) // 👈 Include لجدول الربط هنا أيضاً
        .FirstOrDefaultAsync(g => g.Id == id);

    if (group == null) return null;

    return new GroupDto
    {
      Id = group.Id,
      Name = group.Name,
      CreatedAt = group.CreatedAt,
      Members = group.Members.Select(m => new MemberDto
      {
        Id = m.Id,
        Name = m.Name
      }).ToList(),
      Expenses = group.Expenses.Select(e => new ExpenseDto
      {
        Id = e.Id,
        Title = e.Title,
        Amount = e.Amount,
        PaidById = e.PaidById,
        // 🔥 الحل النهائي: تمرير المعرفات لتصل للأنجولر بنجاح
        ParticipantIds = e.ExpenseShares.Select(es => es.MemberId).ToList()
      }).ToList()
    };
  }
}
