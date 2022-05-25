using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nuyken.VeGasCo.Backend.Domain.Common.Abstractions;

namespace Nuyken.VeGasCo.Backend.Application.Apis.Info.User;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, GetUserInfoResponse>
{
    private readonly IUserAccessor userAccessor;

    public GetUserInfoQueryHandler(IUserAccessor userAccessor)
    {
        this.userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    }

    public Task<GetUserInfoResponse> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        Claim claimFirstName, claimLastName, claimName;
        claimFirstName = userAccessor.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
        claimLastName = userAccessor.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
        claimName = userAccessor.User.Claims.FirstOrDefault(x => x.Type == "name");

        var userInfo = new GetUserInfoResponse
        {
            Id = userAccessor.UserId.ToString(),
            Username = userAccessor.UserName,
            FirstName = claimFirstName?.Value,
            LastName = claimLastName?.Value,
            Name = claimName?.Value
        };
        return Task.FromResult(userInfo);
    }
}