using AutoMapper;
using Models.DTO;
using Models.Models;
using NAV_PRN232_A01.DTOs;

namespace Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SystemAccount, SystemAccountDTO>();
            CreateMap<SystemAccountDTO, SystemAccount>();
            CreateMap<NewsArticleDTO, NewsArticle>()
    .ForMember(dest => dest.Tags, opt => opt.Ignore()) // handled manually
    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
    .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

        }
    }
}
