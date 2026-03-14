using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class CardService(IRepository<Card> cardRepository, IMapper mapper) : ICardService
{
    public async Task<IEnumerable<CardDto>> GetAllAsync()
    {
        var cards = await cardRepository.GetQuery()
            .Include(c => c.Products)
            .Include(c => c.TypeCard)
            .ToListAsync();
        return mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<PagedResponseDto<CardDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = cardRepository.GetQuery()
            .Include(c => c.Products)
            .Include(c => c.TypeCard)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => 
                c.Name.Contains(request.SearchTerm) ||
                c.Description.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var cards = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedCards = mapper.Map<IEnumerable<CardDto>>(cards);

        return new PagedResponseDto<CardDto>
        {
            Data = mappedCards,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<CardDto?> GetByIdAsync(int id)
    {
        var card = await cardRepository.GetQuery()
            .Include(c => c.Products)
            .Include(c => c.TypeCard)
            .FirstOrDefaultAsync(c => c.Id == id);
        return card == null ? null : mapper.Map<CardDto>(card);
    }

    public async Task<CardDto> CreateAsync(CardDto dto)
    {
        var card = mapper.Map<Card>(dto);
        await cardRepository.AddAsync(card);
        return mapper.Map<CardDto>(card);
    }

    public async Task<bool> UpdateAsync(int id, CardDto dto)
    {
        var card = await cardRepository.GetQuery()
            .Include(c => c.Products)
            .Include(c => c.TypeCard)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (card == null) return false;

        card.Name = dto.Name;
        card.Description = dto.Description;
        card.ShelfLifeMonths = dto.ShelfLifeMonths;
        card.PurchaseAmount = dto.PurchaseAmount;
        card.Currency = dto.Currency;
        card.TypeCardId = dto.TypeCardId;
        cardRepository.Update(card);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var card = await cardRepository.GetIdAsync(id);
        if (card == null) return false;

        cardRepository.IsDelete(card);
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await cardRepository.GetQuery()
            .Select(c => new SelectItemDto { Value = c.Id, Text = $"{c.Name} ({c.PurchaseAmount} {c.Currency})" })
            .ToListAsync();
    }
}
