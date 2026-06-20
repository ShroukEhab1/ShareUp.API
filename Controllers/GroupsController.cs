using Microsoft.AspNetCore.Mvc;
using ShareUp.API.DTOs;
using ShareUp.API.Services;

namespace ShareUp.API.Controllers;

[ApiController]
[Route("api/[controller]")] // هتعمل URL زي كده: api/groups
public class GroupsController : ControllerBase
{
  private readonly IExpenseService _expenseService;
  private readonly ISettlementService _settlementService;

  // بنعمل Inject للخدمات اللي عملناها في الخطوة اللي فاتت
  public GroupsController(IExpenseService expenseService, ISettlementService settlementService)
  {
    _expenseService = expenseService;
    _settlementService = settlementService;
  }

  // 1. إنشاء جروب جديد -> POST: api/groups
  [HttpPost]
  public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
  {
    if (string.IsNullOrWhiteSpace(dto.Name))
      return BadRequest("اسم المجموعه مطلوب");

    var result = await _expenseService.CreateGroupAsync(dto);
    return Ok(result);
  }

  // 2. جلب كل الجروبات -> GET: api/groups
  [HttpGet]
  public async Task<IActionResult> GetAllGroups()
  {
    var result = await _expenseService.GetAllGroupsAsync();
    return Ok(result);
  }


  // ⭐ الدالة الجديدة المطلوبة لحل مشكلة الـ 404 ⭐
  // جلب تفاصيل جروب معين بالـ ID -> GET: api/groups/{id}
  [HttpGet("{id}")]
  public async Task<IActionResult> GetGroupById(Guid id)
  {
    // هنا بننادي السيرفيس بتاعتك (اتأكدي إن اسم الدالة مطابق للي عندك في الـ IExpenseService)
    var group = await _expenseService.GetGroupByIdAsync(id);

    if (group == null)
      return NotFound(new { message = $"المجموعة المطلوبة غير موجودة" });

    return Ok(group);
  }
  // 3. إضافة عضو لجروب معين -> POST: api/groups/{groupId}/members
  [HttpPost("{groupId}/members")]
  public async Task<IActionResult> AddMember(Guid groupId, [FromBody] CreateMemberDto dto)
  {
    if (string.IsNullOrWhiteSpace(dto.Name))
      return BadRequest("اسم العضو مطلوب");

    var result = await _expenseService.AddMemberAsync(groupId, dto);
    return Ok(result);
  }

  // 4. تسجيل صرفية جديدة وتقسيمها أوتوماتيك -> POST: api/groups/{groupId}/expenses
  [HttpPost("{groupId}/expenses")]
  public async Task<IActionResult> AddExpense(Guid groupId, [FromBody] CreateExpenseDto dto)
  {
    if (dto.Amount <= 0)
      return BadRequest("المبلغ يجب أن يكون أكبر من صفر");

    if (dto.ParticipantIds == null || dto.ParticipantIds.Count == 0)
      return BadRequest("يجب اختيار عضو واحد على الأقل للمشاركة في المصروف");

    var result = await _expenseService.AddExpenseAsync(groupId, dto);
    return Ok(result);
  }

  // 5. حساب الحسبة النهائية والتصفية (الخوارزمية) -> GET: api/groups/{groupId}/settlement
  [HttpGet("{groupId}/settlement")]
  public async Task<IActionResult> GetSettlement(Guid groupId)
  {
    var result = await _settlementService.CalculateSettlementsAsync(groupId);
    return Ok(result);
  }
}
