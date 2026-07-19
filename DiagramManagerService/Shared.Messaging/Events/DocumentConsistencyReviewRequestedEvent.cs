using System;
using System.Collections.Generic;

namespace Shared.Messaging.Events
{
    public record DocumentConsistencyReviewRequestedEvent(
        Guid DocumentId, 
        List<Guid> DiagramVersionIds
    );
}
