using Microsoft.EntityFrameworkCore;
using ShareUp.API.Data;
using ShareUp.API.DTOs;

namespace ShareUp.API.Services;

public class SettlementService : ISettlementService
{
  private readonly ShareUpDbContext _context;

  public SettlementService(ShareUpDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<TransactionDto>> CalculateSettlementsAsync(Guid groupId)
  {
    // 1. جلب الأعضاء والمصاريف والحصص الخاصة بالجروب من الداتابيز
    var members = await _context.Members
        .Where(m => m.GroupId == groupId)
        .ToListAsync();

    var expenses = await _context.Expenses
        .Where(e => e.GroupId == groupId)
        .ToListAsync();

    var shares = await _context.ExpenseShares
        .Where(es => expenses.Select(e => e.Id).Contains(es.ExpenseId))
        .ToListAsync();

    // 2. قاموس (Dictionary) لحساب صافي الرصيد لكل عضو (دفعه - عليه)
    var balances = members.ToDictionary(m => m.Id, m => 0m);

    // إضافة المبالغ التي دفعها الأعضاء (رصيد موجب)
    foreach (var expense in expenses)
    {
      if (balances.ContainsKey(expense.PaidById))
        balances[expense.PaidById] += expense.Amount;
    }

    // خصم الحصص المستحقة على الأعضاء (رصيد سالب)
    foreach (var share in shares)
    {
      if (balances.ContainsKey(share.MemberId))
        balances[share.MemberId] -= share.ShareAmount;
    }

    // 3. تقسيم الأعضاء إلى دائنين (ليهم فلوس) ومدينين (عليهم فلوس)
    var debtors = balances.Where(b => b.Value < 0).Select(b => new { Id = b.Key, Amount = -b.Value }).ToList();
    var creditors = balances.Where(b => b.Value > 0).Select(b => new { Id = b.Key, Amount = b.Value }).ToList();

    var transactions = new List<TransactionDto>();
    int d = 0, c = 0;

    // 4. خوارزمية التوفيق الذكي تقليص المعاملات
    while (d < debtors.Count && c < creditors.Count)
    {
      var debtor = debtors[d];
      var creditor = creditors[c];

      // المبلغ المتحول هو الأقل بين المديونية والاستحقاق
      decimal minAmount = Math.Min(debtor.Amount, creditor.Amount);

      if (minAmount > 0.01m) // تجاهل الكسور المتناهية الصغر
      {
        var fromMember = members.First(m => m.Id == debtor.Id);
        var toMember = members.First(m => m.Id == creditor.Id);

        transactions.Add(new TransactionDto
        {
          FromMemberId = fromMember.Id,
          FromMemberName = fromMember.Name,
          ToMemberId = toMember.Id,
          ToMemberName = toMember.Name,
          Amount = minAmount
        });
      }

      // تحديث المتبقي
      debtors[d] = new { debtor.Id, Amount = debtor.Amount - minAmount };
      creditors[c] = new { creditor.Id, Amount = creditor.Amount - minAmount };

      if (debtors[d].Amount <= 0.01m) d++;
      if (creditors[c].Amount <= 0.01m) c++;
    }

    return transactions;
  }
}
