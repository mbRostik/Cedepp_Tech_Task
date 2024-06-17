using Cedepp.Application.UseCases.Commands;
using Cedepp.Domain;
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
    public class FinishUserRegistrationHandler : IRequestHandler<FinishUserRegistrationCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly CedeppDbContext dbContext;
        private readonly IPublishEndpoint _publisher;
        private readonly Serilog.ILogger _logger;


        public FinishUserRegistrationHandler(CedeppDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(FinishUserRegistrationCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var unRegUser = await dbContext.UnRegisteredUsers.FirstOrDefaultAsync(x => x.Id == request.userId, cancellationToken);
                if (unRegUser == null)
                {
                    _logger.Warning("Unregistered user not found with Id: {UserId}", request.userId);
                    throw new Exception("Unregistered user not found.");
                }
                _logger.Information("Fetched unregistered user with Id: {UserId}", request.userId);

                var user = new UserModel
                {
                    Id = unRegUser.Id,
                    UserName = unRegUser.UserName,
                    Email = unRegUser.Email,
                    FirstName = request.model.FirstName,
                    LastName = request.model.LastName,
                    PhoneNumber = request.model.PhoneNumber,
                    Address = request.model.Address,
                    CAP = request.model.CAP,
                    CodiceFiscale = request.model.CodiceFiscale,
                    DayOfBirth = request.model.DayOfBirth,
                    Workplace = request.model.Workplace
                };
                _logger.Information("Created new user instance with Id: {UserId}", request.userId);


                var model = await dbContext.Users.AddAsync(user, cancellationToken);
                _logger.Information("User added to the database with Id: {UserId}", request.userId);

                dbContext.UnRegisteredUsers.Remove(unRegUser);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Removed unregistered user and saved changes to the database for UserId: {UserId}", request.userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while finishing user registration for UserId: {UserId}", request.userId);
                return false;
            }
        }
    }
}
