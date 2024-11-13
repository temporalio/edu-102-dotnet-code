using Microsoft.Extensions.Logging;
using Temporalio.Workflows;

namespace TemporalioDurableExecution;

public record TranslationWorkflowInput(string Name, string LanguageCode);
public record TranslationWorkflowOutput(string HelloMessage, string GoodbyeMessage);

[Workflow]
public class TranslationWorkflow
{
    [WorkflowRun]
    public async Task<TranslationWorkflowOutput> RunAsync(TranslationWorkflowInput input)
    {
        var logger = Workflow.Logger;

        logger.LogInformation("TranslationWorkflow invoked with name {Name}", input.Name);

        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(45) };

        logger.LogError("Preparing to translate 'Hello', '{LanguageCode}'", input.LanguageCode);
        var helloInput = new TranslationActivityInput("Hello", input.LanguageCode);

        var helloResult =
            await Workflow.ExecuteActivityAsync((DurableExecutionActivities act) => act.TranslateTermAsync(helloInput), activityOptions);

        var helloMessage = $"{helloResult.Translation}, {input.Name}";

        logger.LogError("Sleeping between translation calls");
        await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

        logger.LogError("Preparing to translate 'Goodbye', '{LanguageCode}'", input.LanguageCode);
        var goodbyeInput = new TranslationActivityInput("Goodbye", input.LanguageCode);

        var byeMessage =
            await Workflow.ExecuteActivityAsync((DurableExecutionActivities act) => act.TranslateTermAsync(goodbyeInput), activityOptions);

        var goodbyeMessage = $"{byeMessage.Translation}, {input.Name}";

        return new TranslationWorkflowOutput(helloMessage, goodbyeMessage);
    }
}