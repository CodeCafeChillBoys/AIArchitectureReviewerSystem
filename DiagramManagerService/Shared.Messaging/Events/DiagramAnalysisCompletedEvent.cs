using System;

namespace Shared.Messaging.Events
{
    public record DiagramAnalysisCompletedEvent(
        Guid DiagramId,
        Guid VersionId,
        float Score,
        string ReviewData,
        string DiagramType = "");
}
