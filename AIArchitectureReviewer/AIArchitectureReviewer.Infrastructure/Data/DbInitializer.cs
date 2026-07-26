using System.Linq;
using System.Threading.Tasks;
using AIArchitectureReviewer.Domain.Entities;
using AIArchitectureReviewer.Application.Prompts;

namespace AIArchitectureReviewer.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedPromptsAsync(ApplicationDbContext context)
        {
            var existing = context.PromptTemplates.ToList();
            if (!existing.Any())
            {
                context.PromptTemplates.AddRange(
                    new PromptTemplate { Name = "Vision Parser", DiagramType = "all", Content = SystemPrompts.VisionParserPrompt },
                    new PromptTemplate { Name = "Auto Refactoring", DiagramType = "all", Content = SystemPrompts.AutoRefactoringPrompt },
                    new PromptTemplate { Name = "Consistency Check", DiagramType = "all", Content = SystemPrompts.ConsistencyCheckPrompt },
                    new PromptTemplate { Name = "Review & Score", DiagramType = "all", Content = SystemPrompts.ReviewAndScorePrompt },
                    new PromptTemplate { Name = "Conformance Review", DiagramType = "all", Content = SystemPrompts.ConformanceReviewPrompt }
                );
            }
            else
            {
                foreach (var p in existing)
                {
                    if (p.Name == "Vision Parser") p.Content = SystemPrompts.VisionParserPrompt;
                    if (p.Name == "Auto Refactoring") p.Content = SystemPrompts.AutoRefactoringPrompt;
                    if (p.Name == "Consistency Check") p.Content = SystemPrompts.ConsistencyCheckPrompt;
                    if (p.Name == "Review & Score") p.Content = SystemPrompts.ReviewAndScorePrompt;
                    if (p.Name == "Conformance Review") p.Content = SystemPrompts.ConformanceReviewPrompt;
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
