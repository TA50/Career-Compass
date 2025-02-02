using AutoMapper;
using CareerCompass.Application.Common;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;
using MediatR;

namespace CareerCompass.Api.Common;

public class ApiControllerContext(ISender sender, IMapper mapper, UserContext userContext)
{
    public readonly ISender Sender = sender;
    public readonly IMapper Mapper = mapper;
    public readonly UserContext UserContext = userContext;
}

public class ApiController(ApiControllerContext context) : ControllerBase
{
    public ApiControllerContext Context { get; } = context;
}