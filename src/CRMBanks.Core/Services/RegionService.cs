using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Infrastructure.Services;

public class RegionService(IRepository<Region> regionRepository, IMapper mapper) : IRegionService
{
    public async Task<IEnumerable<RegionDto>> GetAllAsync()
    {
        var regions = await regionRepository.GetAllAsync();
        return mapper.Map<IEnumerable<RegionDto>>(regions);
    }

    public async Task<PagedResponseDto<RegionDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = regionRepository.GetQuery();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(r => r.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var regions = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedRegions = mapper.Map<IEnumerable<RegionDto>>(regions);

        return new PagedResponseDto<RegionDto>
        {
            Data = mappedRegions,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<RegionDto?> GetByIdAsync(int id)
    {
        var region = await regionRepository.GetIdAsync(id);
        return region == null ? null : mapper.Map<RegionDto>(region);
    }

    public async Task<RegionDto> CreateAsync(RegionDto dto)
    {
        var region = mapper.Map<Region>(dto);
        region.Id = 0;
        var createdRegion = await regionRepository.AddAsync(region);
        return mapper.Map<RegionDto>(createdRegion);
    }

    public async Task<bool> UpdateAsync(int id, RegionDto dto)
    {
        var region = await regionRepository.GetIdAsync(id);
        if (region == null) return false;

        region.Name = dto.Name;
        regionRepository.Update(region);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var region = await regionRepository.GetIdAsync(id);
        if (region == null) return false;

        regionRepository.Remove(region);
        await regionRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await regionRepository.GetQuery()
            .Select(r => new SelectItemDto { Value = r.Id, Text = r.Name })
            .ToListAsync();
    }
}
