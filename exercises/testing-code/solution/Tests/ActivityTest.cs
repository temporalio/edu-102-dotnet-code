namespace TemporalDurableExecution.Tests;

using Temporalio.Testing;
using TemporalioDurableExecution.Workflow;
using Xunit;

internal class TranslationActivityTests
{
    private static readonly HttpClient Client = new HttpClient();

    [Fact]
    public async Task TestSuccessfulTranslateActivityHelloGermanAsync()
    {
        var env = new ActivityEnvironment();
        var input = new Activities.TranslateTermInput("Hello", "de");

        var activities = new Activities(Client);
        var result = await env.RunAsync(() => activities.TranslateTermAsync(input));

        Assert.Equal("hallo", result.Translation);
    }

    [Fact]
    public async Task TestSuccessfulTranslateActivityGoodbyeLatvianAsync()
    {
        var env = new ActivityEnvironment();
        var input = new Activities.TranslateTermInput("Goodbye", "lv");

        var activities = new Activities(Client);
        var result = await env.RunAsync(() => activities.TranslateTermAsync(input));

        Assert.Equal("ardievu", result.Translation);
    }

    [Fact]
    public async Task TestFailedTranslateActivityBadLanguageCodeAsync()
    {
        var env = new ActivityEnvironment();
        var input = new Activities.TranslateTermInput("Hello", "xq");

        var activities = new Activities(Client);

        Task<Activities.TranslateTermOutput> ActAsync() => env.RunAsync(() => activities.TranslateTermAsync(input));

        var exception = await Assert.ThrowsAsync<HttpRequestException>(ActAsync);
        Assert.Contains("Response status code does not indicate success", exception.Message);
    }
}