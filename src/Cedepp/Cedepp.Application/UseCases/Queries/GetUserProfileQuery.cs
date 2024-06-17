using Cedepp.Application.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.UseCases.Queries
{
    public record GetUserProfileQuery(string userId) : IRequest<GiveUserProfileFormDTO>;

}
