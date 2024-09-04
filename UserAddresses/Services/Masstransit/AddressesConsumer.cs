using MassTransit;
using SharedModels.Masstransit.Request;
using SharedModels.Masstransit.Response;
using UserAddresses.Entities;

namespace UserAddresses.Services.Masstransit;

public class AddressesConsumer(AddressesContext addressesContext): IConsumer<IsAddressIDValidRequest>
{
    public async Task Consume(ConsumeContext<IsAddressIDValidRequest> context)
    {
        var address = await addressesContext.Addresses.FindAsync(context.Message.AddressID);
        if (address == null || address.UserId != context.Message.UserId)
        {
            await context.RespondAsync(new IsAddressIDValidResponse() { Success = false });
            return;
        }
        await context.RespondAsync(new IsAddressIDValidResponse() { Success = true });
    }
}