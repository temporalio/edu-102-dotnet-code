using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace TemporalioDurableExecution;

public record TranslationActivityInput(string Term, string LanguageCode);
public record TranslationActivityOutput(string Translation);

public class DurableExecutionActivities
{
    private static readonly HttpClient Client = new();

    [Activity]
    public async Task<TranslationActivityOutput> TranslateTermAsync(TranslationActivityInput input)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("Translating term {Term} to {LanguageCode}", input.Term, input.LanguageCode);

        var lang = Uri.EscapeDataString(input.LanguageCode);
        var term = Uri.EscapeDataString(input.Term);
        var url = $"http://localhost:9998/translate?lang={lang}&term={term}";
        var response = await Client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"HTTP Error {response.StatusCode}: {response.Content}");
        }

        var content = await response.Content.ReadAsStringAsync();
        logger.LogDebug("Translation successful. Translation: {Translation}", content);

        return new TranslationActivityOutput(content);
    }
}