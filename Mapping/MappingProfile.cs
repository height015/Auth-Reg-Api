using System;
using AutoMapper;
using RegAuthApiDemo.Domain;
using RegAuthApiDemo.Models;

namespace RegAuthApiDemo.Mapping
{
	public class MappingProfile : Profile 
	{
		public MappingProfile()
		{
            CreateMap<User, UserModel>().ReverseMap()
				;

            //CreateMap<User, UserModel>().ForAllMembers(
            //   opt => opt.Condition((src, dest, srcMember) => srcMember != null))
            //   ;


            
        }
	}
}

