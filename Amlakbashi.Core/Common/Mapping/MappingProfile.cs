using Amlakbashi.Core.DTOs;
using Amlakbashi.Core.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Features;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;

namespace Amlakbashi.Core.Common.Mapping
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<UserModel, UserViewModel>()
            //    .ForMember(x => x.UserName, opt => opt.MapFrom(y => y.FirstName + " " + y.MiddleName + " " + y.LastName))
            //    .ForMember(x => x.UserAddress, opt => opt.MapFrom(y => y.AddressLine1 + " " + y.AddressLine2 + " " + y.PinCode));
            //CreateMap<BlogPost, BlogPostDTO>()
            //    .ForMember(a => a.UserName, a => a.MapFrom(m => a.fname + " " + a.lname));
            //CreateMap<Ticket, TicketInboxDTO>();
            CreateMap<Service, ServiceDTO>();
        }
    }
}
