using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Common.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Infrastructure.Services;

public class RequestService(
    IRepository<Request> requestRepository,
    IRepository<User> userRepository,
    IRepository<Bank> bankRepository,
    IRepository<RequestAction> requestActionRepository,
    IMapper mapper,
    INotificationService notificationService,
    IEmailService emailService) : IRequestService
{
    public async Task<IEnumerable<RequestDto>> GetAllAsync()
    {
        var requests = await requestRepository.GetQuery()
            .Include(r => r.Region)
            .Include(r => r.Banks)
            .ToListAsync();
        return mapper.Map<IEnumerable<RequestDto>>(requests);
    }

    public async Task<PagedResponseDto<RequestDto>> GetPagedAsync(PagedRequestDto request, int userId)
    {
        var query = requestRepository.GetQuery()
            .Include(r => r.Region)
            .Include(r => r.Banks)
            .AsQueryable();

            var user = await userRepository.GetQuery()
                .Include(u => u.Regions)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user != null)
            {
                switch (user.Role?.Name.ToLower())
                {
                    case "admin":
                        break;
                    case "boss":
                    case "раис":
                        query = query.Where(r => r.Banks!.Any(b => b.Id == user.BankId));
                        break;
                    default:
                    {
                        query = query.Where(r => r.Banks!.Any(b => b.Id == user.BankId));
                    
                        if (user.Regions.Count != 0)
                        {
                            var userRegionIds = user.Regions.Select(r => r.Id).ToList();
                            query = query.Where(r => r.RegionId.HasValue && userRegionIds.Contains(r.RegionId.Value));
                        }
                    
                        if (user.AzSum > 0)
                        {
                            query = query.Where(r => r.Sum >= user.AzSum);
                        }
                        if (user.ToSum > 0)
                        {
                            query = query.Where(r => r.Sum <= user.ToSum);
                        }

                        break;
                    }
                }
            }
        

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(r => 
                r.Name.Contains(request.SearchTerm) ||
                r.Phone.Contains(request.SearchTerm) ||
                r.Email.Contains(request.SearchTerm) ||
                (r.Region != null && r.Region.Name.Contains(request.SearchTerm)));
        }

        var totalCount = await query.CountAsync();
        var requests = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedRequests = mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PagedResponseDto<RequestDto>
        {
            Data = mappedRequests,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<RequestDto?> GetByIdAsync(int id)
    {
        var request = await requestRepository.GetQuery()
            .Include(r => r.Region)
            .Include(r => r.Banks)
            .FirstOrDefaultAsync(r => r.Id == id);
        return request == null ? null : mapper.Map<RequestDto>(request);
    }

    public async Task<RequestDto> CreateAsync(RequestDto dto)
    {
        var banks = await bankRepository.GetQuery().Where(b => dto.BankIds.Contains(b.Id)).ToListAsync();
        var request = new Request
        {
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            ProductId = dto.ProductId,
            Sum = dto.Sum,
            Srok = dto.Srok,
            RegionId = dto.RegionId,
            TypeProductId = dto.TypeProductId,
            Banks = banks,
            State = State.Pending
        };
        var createdRequest = await requestRepository.AddAsync(request);
        await notificationService.CreateForBankWorkersAsync(createdRequest.Id);
        return mapper.Map<RequestDto>(createdRequest);
    }

    public async Task<bool> UpdateStateAsync(int id, string state, string? reason = null, int? userId = null)
    {
        var request = await requestRepository.GetIdAsync(id);
        if (request == null) return false;

        if (!Enum.TryParse<State>(state, true, out var newState))
            return false;

        request.State = newState;
        requestRepository.Update(request);

        if (userId.HasValue)
        {
            await requestActionRepository.AddAsync(new RequestAction
            {
                RequestId = request.Id,
                UserId = userId.Value,
                State = newState,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            });
        }

        await requestRepository.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var stateName = newState switch
            {
                State.Pending => "Дар интизор",
                State.Done => "Иҷро шуд",
                State.Rejected => "Рад шуд",
                State.PastDue => "Муддат гузашт",
                _ => newState.ToString()
            };
            var reasonText = !string.IsNullOrWhiteSpace(reason)
                ? $"<p><b>Сабаб:</b> {reason}</p>"
                : string.Empty;
            try
            {
                emailService.Send(
                    request.Email,
                    "Статуси дархости шумо тағир ёфт",
                    $"<p>Ҳурматли {request.Name},</p>" +
                    $"<p>Статуси дархости шумо ба <b>{stateName}</b> тағир ёфт.</p>" +
                    reasonText +
                    "<p>Бо эҳтиром, Хидмати CRM Банкӣ</p>"
                );
            }
            catch { }
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var request = await requestRepository.GetIdAsync(id);
        if (request == null) return false;

        requestRepository.Remove(request);
        await requestRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await requestRepository.GetQuery()
            .Select(r => new SelectItemDto { Value = r.Id, Text = r.Name })
            .ToListAsync();
    }
}
