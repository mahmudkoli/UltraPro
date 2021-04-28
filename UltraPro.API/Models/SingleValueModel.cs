using UltraPro.Common.Enums;
using UltraPro.Entities;
using UltraPro.API.Mappings;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UltraPro.API.Models
{
    public class SingleValueDetailModel : IMapFrom<SingleValueDetail>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public SingleValueDetailModel()
        {

        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SingleValueDetail, SingleValueDetailModel>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(s => s.Type != null ? s.Type.Name : string.Empty))
                .ForMember(dest => dest.TypeCode, opt => opt.MapFrom(s => s.Type != null ? s.Type.Code : string.Empty));
        }
    }

    public class SaveSingleValueDetailModel : IMapFrom<SingleValueDetail>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public int TypeId { get; set; }

        public SaveSingleValueDetailModel()
        {

        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SaveSingleValueDetailModel, SingleValueDetail>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(s => s.Name.Trim()));
            profile.CreateMap<SingleValueDetail, SaveSingleValueDetailModel>();
        }
    }

    public class SingleValueTypeModel : IMapFrom<SingleValueType>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public SingleValueTypeModel()
        {

        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SingleValueType, SingleValueTypeModel>();
        }
    }
}