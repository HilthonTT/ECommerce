using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Data;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.Users.ChangeUserName;

internal sealed class ChangeUserNameCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ChangeUserNameCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(ChangeUserNameCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound(command.UserId);
        }

        user.ChangeName(command.FirstName, command.LastName);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(user.Id, user.Email, user.FirstName, user.LastName);
    }
}
