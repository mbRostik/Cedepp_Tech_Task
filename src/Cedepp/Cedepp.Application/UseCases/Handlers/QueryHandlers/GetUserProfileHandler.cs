using Cedepp.Application.Contracts.DTOs;
using Cedepp.Application.UseCases.Queries;
using Cedepp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, GiveUserProfileFormDTO>
    {

        private readonly CedeppDbContext dbContext;
        public readonly Serilog.ILogger _logger;

        public GetUserProfileHandler(CedeppDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        public async Task<GiveUserProfileFormDTO> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var checkRegModel = await dbContext.UnRegisteredUsers.FirstOrDefaultAsync(x => x.Id == request.userId, cancellationToken);
                _logger.Information("Checked if user is unregistered for UserId: {UserId}", request.userId);

                if (checkRegModel != null)
                {
                    var unRegResult = new GiveUserProfileFormDTO
                    {
                        UserName = checkRegModel.UserName,
                        IsFinished=false,
                    };
                    _logger.Information("User is unregistered. Returning unregistered user profile for UserId: {UserId}", request.userId);
                    return unRegResult;
                }

                var userModel = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.userId, cancellationToken);

                var result = new GiveUserProfileFormDTO
                {
                    UserName = userModel.UserName,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    PhoneNumber = userModel.PhoneNumber,
                    Address = userModel.Address,
                    CodiceFiscale = userModel.CodiceFiscale,
                    CAP = userModel.CAP,
                    DayOfBirth = userModel.DayOfBirth,
                    Workplace = userModel.Workplace,
                    Photo = userModel.Photo,
                    IsFinished = true
                };

                _logger.Information("Handle method completed successfully for UserId: {UserId}", request.userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching user profile for UserId: {UserId}", request.userId);
                return null;
            }
        }
    }
}
