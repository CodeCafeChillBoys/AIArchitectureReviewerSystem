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
            if (!context.PromptTemplates.Any())
            {
                context.PromptTemplates.AddRange(
                    new PromptTemplate { Name = "Vision Parser", DiagramType = "all", Content = SystemPrompts.VisionParserPrompt },
                    new PromptTemplate { Name = "Auto Refactoring", DiagramType = "all", Content = SystemPrompts.AutoRefactoringPrompt },
                    new PromptTemplate { Name = "Consistency Check", DiagramType = "all", Content = SystemPrompts.ConsistencyCheckPrompt },
                    new PromptTemplate { Name = "Review & Score", DiagramType = "all", Content = SystemPrompts.ReviewAndScorePrompt },
                    new PromptTemplate { Name = "Conformance Review", DiagramType = "all", Content = SystemPrompts.ConformanceReviewPrompt }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
