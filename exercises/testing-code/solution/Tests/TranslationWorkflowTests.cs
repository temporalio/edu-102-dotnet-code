namespace TemporalDurableExecution.Workflow;

using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using TemporalioDurableExecution.Workflow;
using Xunit;

internal class TranslationWorkflowTests
{
    private static readonly HttpClient Client = new HttpClient();

    [Fact]
    public async Task TestSuccessfulCompleteFrenchTranslationAsync()
    {
        var taskQueueId = Guid.NewGuid().ToString();
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();

        var activities = new Activities(Client);

        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueueId)
                .AddWorkflow<TranslationWorkflow>()
                .AddActivity(activities.TranslateTermAsync));

        await worker.ExecuteAsync(async () =>
        {
            var input = new TranslationWorkflow.TranslationWorkflowInput("Pierre", "fr");
            var result = await env.Client.ExecuteWorkflowAsync(
                (TranslationWorkflow wf) => wf.RunAsync(input),
                new(id: $"wf-{Guid.NewGuid()}", taskQueue: taskQueueId));

            Assert.Equal("bonjour, Pierre", result.HelloMessage);
            Assert.Equal("au revoir, Pierre", result.GoodbyeMessage);
        });
    }
}