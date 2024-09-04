using Login.Entities;
using MassTransit;
using SharedModels.Masstransit.Request;
using SharedModels.Masstransit.Response;


namespace Login.Services.Masstransit;

public class UserConsumer(LoginContext loginContext) : IConsumer<DeleteUserRequest>
{
    public async Task Consume(ConsumeContext<DeleteUserRequest> context)
    {
        // Delete the user
        var user = await loginContext.Users.FindAsync(context.Message.UserId);
        if (user == null)
        {
            await context.RespondAsync(new DeleteUserResponse() { Success = false });
            return;
        }
        loginContext.Users.Remove(user);
        await context.RespondAsync(new DeleteUserResponse() { Success = true });
    }
}