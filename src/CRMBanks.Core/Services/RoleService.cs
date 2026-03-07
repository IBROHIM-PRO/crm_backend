using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Infrastructure.Services;

public class RoleService(IRepository<Role> roleRepository) : IRoleService
{
    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        return await roleRepository.GetQuery()
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
            .ToListAsync();
    }

    public async Task<PagedResponseDto<RoleDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = roleRepository.GetQuery();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(r => r.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync();
        var roles = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
            .ToListAsync();

        return new PagedResponseDto<RoleDto>
        {
            Data = roles,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await roleRepository.GetIdAsync(id);
        return role == null ? null : new RoleDto { Id = role.Id, Name = role.Name };
    }

    public async Task<RoleDto> CreateAsync(RoleDto dto)
    {
        var role = new Role { Name = dto.Name };
        var createdRole = await roleRepository.AddAsync(role);
        return new RoleDto { Id = createdRole.Id, Name = createdRole.Name };
    }

    public async Task<bool> UpdateAsync(int id, RoleDto dto)
    {
        var role = await roleRepository.GetIdAsync(id);
        if (role == null) return false;

        role.Name = dto.Name;
        roleRepository.Update(role);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await roleRepository.GetIdAsync(id);
        if (role == null) return false;

        roleRepository.Remove(role);
        await roleRepository.SaveChangesAsync();
        return true;
    }
}
