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
    public class ChangeUserPhotoHandler : IRequestHandler<ChangeProfilePhotoCommand>
    {
        private readonly IMediator mediator;

        private readonly CedeppDbContext dbContext;
        private readonly Serilog.ILogger _logger;

        public ChangeUserPhotoHandler(CedeppDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ChangeProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to change avatar for user ID: {UserId}", request.Id);

            try
            {
                var user = await dbContext.Users.FindAsync(new object[] { request.Id }, cancellationToken);
                if (user != null)
                {
                    user.Photo = Convert.FromBase64String(request.model.Photo);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    _logger.Information("Avatar changed successfully for user ID: {UserId}", request.Id);
                }
                else
                {
                    _logger.Warning("User with ID: {UserId} not found. Cannot change avatar.", request.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while changing avatar for user ID: {UserId}", request.Id);
                throw;
            }
        }
    }
}