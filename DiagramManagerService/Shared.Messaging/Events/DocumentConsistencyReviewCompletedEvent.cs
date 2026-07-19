using System;

namespace Shared.Messaging.Events
{
    public record DocumentConsistencyReviewCompletedEvent(
        Guid DocumentId,
        float ConsistencyScore,
        string ConsistencyReviewData
    );
}
