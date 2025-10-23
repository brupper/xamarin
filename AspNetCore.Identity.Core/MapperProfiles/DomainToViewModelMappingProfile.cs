using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Identity.Models;
using AutoMapper;

namespace Brupper.AspNetCore.Identity.MapperProfiles;

public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        CreateMapForEntities();

        CreateMapForUser();
        CreateMapForTenant();
    }

    private void CreateMapForUser()
    {
        CreateMap<User, UserEditViewModel>()
            .ReverseMap()
           ;
    }

    private void CreateMapForTenant()
    {
        CreateMap<Tenant, TenantViewModel>()
           .ForMember(dest => dest.PrimaryKey, opt => opt.MapFrom(p => p.Id))
           .ForMember(dest => dest.Modules, opt => opt.MapFrom(p => p.Licences.Select(x => new TenantModulesViewModel { Name = x.Name, Selected = true }).ToList()))
           ;

        CreateMap<TenantViewModel, Tenant>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(p => p.PrimaryKey))
           .ForMember(dest => dest.Licences, opt => opt.MapFrom(p => p.Modules.Select(x => new ModuleReference { Name = x.Name }).ToList()))
           ;

        CreateMap<TenantModulesViewModel, ModuleReference>()
            .ReverseMap();
    }

    private void CreateMapForEntities()
    {
        CreateMap<Tenant, Tenant>();
        CreateMap<User, User>();
    }
}
