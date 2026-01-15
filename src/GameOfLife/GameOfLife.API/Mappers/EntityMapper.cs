using AutoMapper;
using GameOfLife.API.Models;
using JetBrains.Annotations;

namespace GameOfLife.API.Mappers
{
    [UsedImplicitly]
    public class EntityMapper : Profile
    {
        public EntityMapper()
        {
            CreateMap<Coordinate, Entity>()
                .ConstructUsing(c => new Entity(c.X, c.Y));

            CreateMap<Entity, Coordinate>()
                .ForMember(dest => dest.X, opt => opt.MapFrom(src => src.XCoordinate))
                .ForMember(dest => dest.Y, opt => opt.MapFrom(src => src.YCoordinate));
        }
    }
}
