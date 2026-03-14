using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class TypeProductService(IRepository<TypeProduct> typeProductRepository,
    IRepository<TypeCredit> typeCreditRepository,
    IRepository<TypeDeposit> typeDepositRepository,
    IMapper mapper) : ITypeProductService
{
    public async Task<IEnumerable<TypeProductDto>> GetAllAsync()
    {
        var typeProducts = await typeProductRepository.GetQuery()
            .Include(tp => tp.TypeCredit)
            .Include(tp => tp.TypeDeposit)
            .ToListAsync();
        return mapper.Map<IEnumerable<TypeProductDto>>(typeProducts);
    }

    public async Task<PagedResponseDto<TypeProductDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = typeProductRepository.GetQuery()
            .Include(tp => tp.TypeCredit)
            .Include(tp => tp.TypeDeposit)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var typeProducts = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedTypeProducts = mapper.Map<IEnumerable<TypeProductDto>>(typeProducts);

        return new PagedResponseDto<TypeProductDto>
        {
            Data = mappedTypeProducts,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<TypeProductDto?> GetByIdAsync(int id)
    {
        var typeProduct = await typeProductRepository.GetQuery()
            .Include(tp => tp.TypeCredit)
            .Include(tp => tp.TypeDeposit)
            .FirstOrDefaultAsync(tp => tp.Id == id);
        return typeProduct == null ? null : mapper.Map<TypeProductDto>(typeProduct);
    }

    public async Task<TypeProductDto> CreateAsync(TypeProductDto dto)
    {
        var typeProduct = mapper.Map<TypeProduct>(dto);
        await typeProductRepository.AddAsync(typeProduct);
        return mapper.Map<TypeProductDto>(typeProduct);
    }

    public async Task<bool> UpdateAsync(int id, TypeProductDto dto)
    {
        var typeProduct = await typeProductRepository.GetQuery()
            .Include(tp => tp.TypeCredit)
            .Include(tp => tp.TypeDeposit)
            .FirstOrDefaultAsync(tp => tp.Id == id);
        if (typeProduct == null) return false;

        typeProduct.TypeCreditId = dto.TypeCreditId;
        typeProduct.TypeDepositId = dto.TypeDepositId;

        typeProductRepository.Update(typeProduct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var typeProduct = await typeProductRepository.GetIdAsync(id);
        if (typeProduct == null) return false;

        typeProductRepository.IsDelete(typeProduct);
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await typeProductRepository.GetQuery()
            .Select(tp => new SelectItemDto { Value = tp.Id, Text = $"TypeProduct {tp.Id}" })
            .ToListAsync();
    }
}
