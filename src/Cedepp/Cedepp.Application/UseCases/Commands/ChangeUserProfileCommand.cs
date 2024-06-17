using Cedepp.Application.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.UseCases.Commands
{
    public record ChangeUserProfileCommand(ChangeUserProfileDTO model, string Id) : IRequest<bool>;

}
