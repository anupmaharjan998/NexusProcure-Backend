using AutoMapper;
using NexusProcure.Application.DTOs;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null));

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // handled manually

        // // Role
        // CreateMap<Role, RoleDto>()
        //     .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission.Name)));
        //
        // CreateMap<CreateRoleDto, Role>();
        //
        // // Department
        // CreateMap<Department, DepartmentDto>()
        //     .ForMember(dest => dest.HeadName, opt => opt.MapFrom(src => src.Head != null ? src.Head.Username : null));
        //
        // CreateMap<CreateDepartmentDto, Department>();
        //
        // // Permission
        // CreateMap<Permission, PermissionDto>();
    }
}