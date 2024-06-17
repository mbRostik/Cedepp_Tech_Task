using Cedepp.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.UseCases.Commands
{
    public record CreateUnRegisteredUserCommand(UnRegisteredUser model) : IRequest;

}
