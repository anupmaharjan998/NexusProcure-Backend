using AutoMapper;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.DTOs.Vendor;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Id))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Department != null ? src.Department.Id : (Guid?)null))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null));

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // handled manually

        // // Role
        CreateMap<Role, RoleDto>();
        
        CreateMap<CreateRoleDto, Role>();
        
        // Department
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.HeadName, opt => opt.MapFrom(src => src.Head != null ? src.Head.FullName : null))
            .ForMember(dest => dest.EmployeesCount, opt => opt.MapFrom(src => src.Users != null ? src.Users.Count : 0));
        
        CreateMap<CreateDepartmentDto, Department>();
        
        // // Permission
        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Description));
        
        // Vendor
        CreateMap<VendorRequestDto, Vendor>();
        CreateMap<Vendor, VendorResponseDto>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category != null ? src.Category.Id : (Guid?)null));
        CreateMap<VendorDocument, VendorDocumentResponseDto>();
        
        CreateMap<Category, CategoryResponse>();
        CreateMap<CategoryRequest, Category>();
        
        // RequisitionItem -> RequisitionItemDto
        CreateMap<RequisitionItem, RequisitionItemDto>();

// Requisition -> RequisitionResponseDto
        CreateMap<Requisition, RequisitionResponseDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

// Optionally, also map nested PurchaseOrderItem if needed
        // CreateMap<PurchaseOrder, PurchaseOrderDto>()
        //     .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>();

        //CreateMap<CreateRoleDto, Role>();
        
        
        CreateMap<ApprovalLevel, ApprovalLevelResponseDto>();
    }
}