using SaaSBillingApi.Domain.Enums;

namespace SaaSBillingApi.Domain.Exceptions;

public class InvalidSubscriptionTransitionException : DomainException
{
    public SubscriptionStatus From { get; }
    public SubscriptionStatus To { get; }

    public InvalidSubscriptionTransitionException(SubscriptionStatus from, SubscriptionStatus to)
        : base($"Cannot transition subscription from '{from}' to '{to}'.")
    {
        From = from;
        To = to;
    }
}