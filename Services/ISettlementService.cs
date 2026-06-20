using ShareUp.API.DTOs;

namespace ShareUp.API.Services;

public interface ISettlementService
{
  Task<IEnumerable<TransactionDto>> CalculateSettlementsAsync(Guid groupId);
}
