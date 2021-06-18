using UltraPro.Entities;
using UltraPro.API.Mappings;
using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace UltraPro.API.Models
{
    public class ApiLoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ApiResetPasswordModel  
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string Token { get; set; } 
    }

    public class ApiAppLoginModel
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ApiAppChangePasswordModel
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]

        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class ApiAppRegistrationModel : IMapFrom<ApplicationUser>
    {
        [Required]
        public string FullName { get; set; }
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(01[0-9]{9})$", ErrorMessage = "Please, Provide 11 digit phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApiAppRegistrationModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.PhoneNumber.Trim()))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Trim()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim()));
        }
    }
    
    public class ApiAuthenticateUserModel : IMapFrom<ApplicationUser>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }
        public string JwtToken { get; set; }
        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApplicationUser, ApiAuthenticateUserModel>();
        }
    }

    public class ApiAppRevokeTokenModel
    {
        public string Token { get; set; }
    }
}