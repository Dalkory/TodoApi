using AutoMapper;
using TodoApi.Core.Entities;
using TodoApi.Web.DTOs;

namespace TodoApi.Infrastructure.Mapping
{
    public class TodoProfile : Profile
    {
        public TodoProfile()
        {
            CreateMap<TodoItem, TodoDto>();
            CreateMap<CreateTodoDto, TodoItem>()
                .ConstructUsing(dto => TodoItem.Create(dto.Title));
            CreateMap<UpdateTodoDto, TodoItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}