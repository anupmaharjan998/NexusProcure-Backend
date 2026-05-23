using AutoMapper;
using NexusProcure.Application.Services.Procurement;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.DTOs.RFQ;
using NexusProcure.Core.DTOs.Vendor;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Entities.RequestForQuotations;

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
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null))
            .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.Manager.Id))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.FullName));

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
            .ForMember(dest => dest.CategoryIds,
                opt => opt.MapFrom(src => src.VendorCategories.Select(vc => vc.CategoryId).ToList()))
            .ForMember(dest => dest.CategoryNames,
                opt => opt.MapFrom(src => src.VendorCategories.Select(vc => vc.Category.Name).ToList()));
        
        CreateMap<VendorDocument, VendorDocumentResponseDto>();
        
        CreateMap<Category, CategoryResponse>();
        CreateMap<CategoryRequest, Category>();
        
        // RequisitionItem -> RequisitionItemDto
        CreateMap<RequisitionItem, RequisitionItemDto>();

// Requisition -> RequisitionResponseDto
        CreateMap<Requisition, RequisitionResponseDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.RequestedBy.FullName))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        
        CreateMap<User, UserResponseDto>();
        
        
// Quotation -> QuotationResponseDto
        CreateMap<Quotation, QuotationApprovalListResponseDto>()
            .ForMember(dest => dest.RfqNumber, opt => opt.MapFrom(src => src.RequestForQuotation.RfqNumber))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.RfqVendor.Vendor.CompanyName))
            .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.RfqVendor.Vendor.VendorName))
            .ForMember(dest => dest.RfqNumber, opt => opt.MapFrom(src => src.RequestForQuotation.RfqNumber));


// Optionally, also map nested PurchaseOrderItem if needed
        // CreateMap<PurchaseOrder, PurchaseOrderDto>()
        //     .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>();

        //CreateMap<CreateRoleDto, Role>();
        
        
        CreateMap<ApprovalLevel, ApprovalLevelResponseDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
        
        
        CreateMap<Approval, ApprovalDto>();

        CreateMap<RequestForQuotation, RfqDto>()
            .ForMember(
                dest => dest.TotalQuotationsRecieved,
                opt => opt.MapFrom(src => src.Quotations.Count)
            );
        
        
        
        
        //Quotation Details
        CreateMap<Quotation, QuotationDetailsDto>()
            .ForMember(
                dest => dest.Items,
                opt => opt.MapFrom(src => src.Items)
            ).ForMember(
                dest => dest.VendorName,
                opt => opt.MapFrom(src => src.RfqVendor.Vendor.CompanyName)
            ).ForMember(
                dest => dest.VendorEmail,
                opt => opt.MapFrom(src => src.RfqVendor.Vendor.Email)
            ).ForMember(
                dest => dest.ContactPerson,
                opt => opt.MapFrom(src => src.RfqVendor.Vendor.VendorName)
            ).ForMember(
                dest => dest.VendorPhone,
                opt => opt.MapFrom(src => src.RfqVendor.Vendor.PhoneNumber)
            ).ForMember(
                dest => dest.VendorAddress,
                opt => opt.MapFrom(src => src.RfqVendor.Vendor.Address)
            );
        
        CreateMap<QuotationItem, QuotationItemsDto>()
            .ForMember(
                dest => dest.Total,
                opt => opt.MapFrom(src =>
                    src.Quantity * src.UnitPrice * (1 + src.TaxPercentage / 100)
                )
            );


    }
}