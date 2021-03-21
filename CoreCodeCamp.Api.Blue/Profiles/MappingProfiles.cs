using AutoMapper;
using CoreCodeCamp.Api.Blue.Data;
using CoreCodeCamp.Api.Blue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Api.Blue.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Camp, CampModel>().ForMember(dist => dist.Venue, op => op.MapFrom(src => src.Location.VenueName)).ReverseMap();
            CreateMap<Talk, TalkModel>().ReverseMap();
            CreateMap<Speaker, SpeakerModel>().ReverseMap();

        }
    }
}
