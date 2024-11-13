// This file is designated to run the Workflow
using Microsoft.Extensions.Logging;
using Temporalio.Client;
using TemporalioDurableExecution;

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new("localhost:7233")
{
    LoggerFactory = LoggerFactory.Create(builder =>
        builder.
            AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").
            SetMinimumLevel(LogLevel.Information)),
});
var input = new TranslationWorkflowInput(args[0], args[1]);
var options = new WorkflowOptions(
            id: "translation-workflow",
            taskQueue: WorkflowConstants.TaskQueueName);

// Run workflow
var result = await client.ExecuteWorkflowAsync(
    (TranslationWorkflow wf) => wf.RunAsync(input),
    options);

Console.WriteLine($"Workflow result: {result}");
