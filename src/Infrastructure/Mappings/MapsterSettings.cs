using Mapster;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Infrastructure.Mappings;

public class MapsterSettings
{
    public static void Configure()
    {
        // here we will define the type conversion / Custom-mapping
        // More details at https://github.com/MapsterMapper/Mapster/wiki/Custom-mapping
        TypeAdapterConfig<ApplicationRoleClaim, PermissionDto>.NewConfig().Map(dest => dest.Permission, src => src.ClaimValue);
    }
}
