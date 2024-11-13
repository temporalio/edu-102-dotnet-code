// // This file is designated to run the Worker
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Temporalio.Client;
using Temporalio.Extensions.Hosting;
using Temporalio.Worker;
using TemporalioDurableExecution;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(ctx => ctx.AddSimpleConsole().SetMinimumLevel(LogLevel.Information))
    .ConfigureServices(ctx =>
        ctx.
            // Add the worker
            AddHostedTemporalWorker(
                clientTargetHost: "localhost:7233",
                clientNamespace: "default",
                taskQueue: WorkflowConstants.TaskQueueName).
            // Add the activities class at the scoped level
            AddScopedActivities<DurableExecutionActivities>().
            AddWorkflow<TranslationWorkflow>())
    .Build();

await host.RunAsync();