using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Data;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Application.Users;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.Authentication.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IIdentityProviderService identityProviderService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        Result<string> result = await identityProviderService.RegisterUserAsync(
            new UserModel(
                command.Email,
                command.Password,
                command.FirstName,
                command.LastName),
            cancellationToken);

        if (result.IsFailure)
        {
            return result.Error;
        }

        var user = User.Create(command.Email, command.FirstName, command.LastName, result.Value);

        userRepository.Insert(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(user.Id, user.Email, user.FirstName, user.LastName);
    }
}
