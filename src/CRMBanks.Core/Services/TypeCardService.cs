using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class TypeCardService(IRepository<TypeCard> typeCardRepository, IMapper mapper) : ITypeCardService
{
    public async Task<IEnumerable<TypeCardDto>> GetAllAsync()
    {
        var typeCards = await typeCardRepository.GetQuery()
            .Include(tc => tc.Cards)
            .ToListAsync();
        return mapper.Map<IEnumerable<TypeCardDto>>(typeCards);
    }

    public async Task<PagedResponseDto<TypeCardDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = typeCardRepository.GetQuery()
            .Include(tc => tc.Cards)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(tc => tc.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var typeCards = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedTypeCards = mapper.Map<IEnumerable<TypeCardDto>>(typeCards);

        return new PagedResponseDto<TypeCardDto>
        {
            Data = mappedTypeCards,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<TypeCardDto?> GetByIdAsync(int id)
    {
        var typeCard = await typeCardRepository.GetQuery()
            .Include(tc => tc.Cards)
            .FirstOrDefaultAsync(tc => tc.Id == id);
        return typeCard == null ? null : mapper.Map<TypeCardDto>(typeCard);
    }

    public async Task<TypeCardDto> CreateAsync(TypeCardDto dto)
    {
        var typeCard = mapper.Map<TypeCard>(dto);
        await typeCardRepository.AddAsync(typeCard);
        return mapper.Map<TypeCardDto>(typeCard);
    }

    public async Task<bool> UpdateAsync(int id, TypeCardDto dto)
    {
        var typeCard = await typeCardRepository.GetQuery()
            .Include(tc => tc.Cards)
            .FirstOrDefaultAsync(tc => tc.Id == id);
        if (typeCard == null) return false;

        typeCard.Name = dto.Name;

        typeCardRepository.Update(typeCard);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var typeCard = await typeCardRepository.GetIdAsync(id);
        if (typeCard == null) return false;

        typeCardRepository.IsDelete(typeCard);
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await typeCardRepository.GetQuery()
            .Select(tc => new SelectItemDto { Value = tc.Id, Text = tc.Name })
            .ToListAsync();
    }
}
