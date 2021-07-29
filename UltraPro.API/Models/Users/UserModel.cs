using UltraPro.Common.Enums;
using UltraPro.Entities;
using UltraPro.Common.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace UltraPro.API.Models
{
    public class UserModel : IMapFrom<ApplicationUser>
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string FullName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        //public string ImageUrl { get; set; }
        public Guid? UserRoleId { get; set; }

        public string UserRoleName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public EnumApplicationUserStatus? Status { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApplicationUser, UserModel>()
                .ForMember(dest => dest.UserRoleName, opt => opt.MapFrom(src =>
                    src.UserRoles.Any() ? (src.UserRoles.Select(ur => ur.Role).FirstOrDefault() != null ?
                    src.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault() : null) : null))
                .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src =>
                    src.UserRoles.Any() ? (src.UserRoles.Select(ur => ur.Role).FirstOrDefault() != null ?
                    src.UserRoles.Select(ur => ur.RoleId).FirstOrDefault() : (Guid?)null) : (Guid?)null));
        }
    }

    public class SaveUserModel : IMapFrom<ApplicationUser>
    {
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        //public string ImageUrl { get; set; }

        [Required]
        public Guid UserRoleId { get; set; }
        //public IFormFile InputFile { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SaveUserModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(s => s.UserName.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(s => s.Email.Trim()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(s => s.FullName.Trim()))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Trim()));
            profile.CreateMap<ApplicationUser, SaveUserModel>()
                .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(s => s.UserRoles.Any() ? 
                    s.UserRoles.FirstOrDefault().RoleId : Guid.Empty));
        }
    }
}