using System;

namespace Shared.Messaging.Events
{
    public record DiagramUploadedEvent(Guid DiagramId, Guid VersionId, string FileUrl, byte[] FileData);
}
