using Cedepp.Application.UseCases.Commands;
using Cedepp.Infrastructure.Data;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.UseCases.Handlers.OperationHandlers
{
    public class ChangeUserProfileHandler : IRequestHandler<ChangeUserProfileCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly CedeppDbContext dbContext;
        private readonly IPublishEndpoint _publisher;
        public readonly Serilog.ILogger _logger;

        public ChangeUserProfileHandler(CedeppDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangeUserProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (user == null)
                {
                    _logger.Warning("User not found with Id: {UserId}", request.Id);
                    return false;
                }
                _logger.Information("Fetched user with Id: {UserId}", request.Id);

                user.FirstName = request.model.FirstName;
                user.LastName = request.model.LastName;
                user.PhoneNumber = request.model.PhoneNumber;
                user.CAP = request.model.CAP;
                user.CodiceFiscale = request.model.CodiceFiscale;
                user.DayOfBirth = request.model.DayOfBirth;
                user.Workplace = request.model.Workplace;
                _logger.Information("The info about user was changed and saved: {UserId}", request.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while changing profile for UserId: {UserId}", request.Id);
                return false;
            }
        }
    }
}
