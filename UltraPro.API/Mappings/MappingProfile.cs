using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Mappings;
using UltraPro.Common.Model;

namespace UltraPro.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CommonMappingProfile.ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly(), this);
            CreateMap(typeof(QueryResult<>), typeof(QueryResult<>));
        }
    }
}
