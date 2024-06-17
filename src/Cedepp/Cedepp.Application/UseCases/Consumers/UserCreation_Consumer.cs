using Cedepp.Application.UseCases.Commands;
using Cedepp.Domain;
using MassTransit;
using MediatR;
using MessageBus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<UserCreationEvent>
    {
        private readonly IMediator mediator;
        public readonly Serilog.ILogger _logger;

        public UserCreation_Consumer(IMediator _mediator, Serilog.ILogger logger)
        {
            mediator = _mediator;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<UserCreationEvent> context)
        {
            _logger.Information("Successfully consumed UserCreationEvent");

            UnRegisteredUser temp = new UnRegisteredUser
            {
                Id = context.Message.UserId,
                UserName = context.Message.UserName,
                Email = context.Message.Email
            };
            await mediator.Send(new CreateUnRegisteredUserCommand(temp));
        }
    }
}
