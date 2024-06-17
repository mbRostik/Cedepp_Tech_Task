using Cedepp.Application.UseCases.Commands;
using Cedepp.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.UseCases.Handlers.OperationHandlers
{
    public class UnRegisteredUserCreateHandler : IRequestHandler<CreateUnRegisteredUserCommand>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger _logger;

        private readonly CedeppDbContext dbContext;

        public UnRegisteredUserCreateHandler(CedeppDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            _logger = logger;
        }


        public async Task Handle(CreateUnRegisteredUserCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var model = await dbContext.UnRegisteredUsers.AddAsync(request.model, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Unregistered user created successfully with Id: {UserId}", request.model.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating unregistered user with Id: {UserId}", request.model.Id);
                throw;
            }
        }
    }
}
