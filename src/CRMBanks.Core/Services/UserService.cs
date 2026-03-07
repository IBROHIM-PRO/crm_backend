using AutoMapper;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class UserService(IRepository<User> userRepository,
    IRepository<Region> regionRepository,
    IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetQuery()
            .Include(u => u.Bank)
            .Include(u => u.Role)
            .Include(u => u.Regions)
            .ToListAsync();
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<PagedResponseDto<UserDto>> GetPagedAsync(PagedRequestDto request)
    {
        var query = userRepository.GetQuery()
            .Include(u => u.Bank)
            .Include(u => u.Role)
            .Include(u => u.Regions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(u => 
                u.Name.Contains(request.SearchTerm) ||
                u.Phone.Contains(request.SearchTerm) ||
                u.Email.Contains(request.SearchTerm) ||
                (u.Bank != null && u.Bank.Name.Contains(request.SearchTerm)) ||
                (u.Role != null && u.Role.Name.Contains(request.SearchTerm)));
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var mappedUsers = mapper.Map<IEnumerable<UserDto>>(users);

        return new PagedResponseDto<UserDto>
        {
            Data = mappedUsers,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await userRepository.GetQuery()
            .Include(u => u.Bank)
            .Include(u => u.Role)
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? null : mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(UserDto dto)
    {
        var regions = await regionRepository.GetQuery().Where(r => dto.RegionIds.Contains(r.Id)).ToListAsync();
        var user = mapper.Map<User>(dto);
        user.Regions = regions;
        user.Token = Guid.NewGuid().ToString();
        await userRepository.AddAsync(user);
        return mapper.Map<UserDto>(user);
    }

    public async Task<bool> UpdateAsync(int id, UserDto dto)
    {
        var user = await userRepository.GetQuery()
            .Include(u => u.Bank)
            .Include(u => u.Role)
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return false;

        user.Name = dto.Name;
        user.Phone = dto.Phone;
        user.BankId = dto.BankId;
        user.RoleId = dto.RoleId;
        user.AzSum = dto.AzSum;
        user.ToSum = dto.ToSum;

        // Handle regions update similar to attributes/images pattern
        if (dto.RegionIds.Count != 0)
        {
            var regionsFromDto = await regionRepository.GetQuery()
                .Where(r => dto.RegionIds.Contains(r.Id))
                .ToListAsync();

            user.Regions ??= new List<Region>();

            var toRemove = user.Regions
                .Where(r => !dto.RegionIds.Contains(r.Id))
                .ToList();

            foreach (var region in toRemove)
                user.Regions.Remove(region);

            var toAdd = regionsFromDto
                .Where(r => !user.Regions.Any(ur => ur.Id == r.Id))
                .ToList();

            user.Regions.AddRange(toAdd);
        }

        userRepository.Update(user);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await userRepository.GetIdAsync(id);
        if (user == null) return false;

        userRepository.Remove(user);
        return true;
    }

    public async Task<IEnumerable<SelectItemDto>> GetSelectListAsync()
    {
        return await regionRepository.GetQuery()
            .Select(u => new SelectItemDto { Value = u.Id, Text = u.Name })
            .ToListAsync();
    }
}
