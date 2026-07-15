namespace AIArchitectureReviewer.Application.Interfaces.Prompt
{
    public interface IPromptBuilder
    {
        IPromptBuilder AddSystemContext(string context);
        IPromptBuilder AddUserInstruction(string instruction);
        IPromptBuilder AddData(string key, string data);
        string Build();
    }
}
