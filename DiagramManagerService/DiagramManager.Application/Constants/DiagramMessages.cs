namespace DiagramManager.Application.Constants;

public static class DiagramMessages
{
    public const string OperationSuccess = "Operation completed successfully.";
    public const string WorkspaceCreatedSuccess = "Workspace created successfully.";
    public const string WorkspaceUpdatedSuccess = "Workspace updated successfully.";
    public const string WorkspaceNotFound = "Workspace not found.";
    public const string DiagramCreatedSuccess = "Diagram created successfully.";
    public const string DiagramNotFound = "Diagram not found.";

    // Validation messages
    public const string InvalidWorkspaceName = "Workspace name is required and must be between 2 and 200 characters.";
    public const string InvalidUserId = "User ID is required and cannot be empty.";
    public const string DuplicateWorkspaceName = "A workspace with this name already exists for this user.";
}