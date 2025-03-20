// See https://aka.ms/new-console-template for more information
using System.Text.Json;

var content = await GetContentFromUrlAsync("https://bigconference.azurewebsites.net/sessions");
var sessions = ParseSessions(content);
PrintSessions(sessions);

async Task<string> GetContentFromUrlAsync(string url)
{
    using var client = new HttpClient();
    string? key = "sdssdsdsd";
    var response = await client.GetAsync(url);
    client.DefaultRequestHeaders.Add("authorization", key);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}

List<ConferenceSession> ParseSessions(string content)
{
    JsonDocument jsonDocument = JsonDocument.Parse(content);

    var sessions = new List<ConferenceSession>();

    JsonElement root = jsonDocument.RootElement;
    JsonElement sessionsElement = root.GetProperty("collection").GetProperty("items");
    if (sessionsElement.ValueKind != JsonValueKind.Array)
    {
        Console.WriteLine("No sessions found");
        return sessions;
    }
    else
    {
        foreach (JsonElement session in sessionsElement.EnumerateArray())
        {
            string? title = string.Empty;
            string? timeslot = string.Empty;
            string? speaker  = string.Empty;
            string link = session.GetProperty("href").GetString()?.Trim() ?? string.Empty;
            foreach (JsonElement dataElement in session.GetProperty("data").EnumerateArray())
            {
                string name = dataElement.GetProperty("name").GetString()?.Trim() ?? string.Empty;
                string value = dataElement.GetProperty("value").GetString()?.Trim() ?? string.Empty;

                switch (name)
                {
                    case "Title":
                        title = value;
                        break;
                    case "Timeslot":
                        timeslot = value;
                        break;
                    case "Speaker":
                        speaker = value ?? "TBA";
                        break;
                }
            }

            sessions.Add(new ConferenceSession
            {
                Title = title ?? string.Empty,
                Timeslot = timeslot ?? string.Empty,
                Speaker = speaker,
                Link = link ?? string.Empty
            });
        }
    }

    return sessions;
}

void PrintSessions(List<ConferenceSession> sessions)
{
    Console.WriteLine($"Conference Sessions ({sessions.Count}):");
    foreach (var session in sessions)
    {
        Console.WriteLine($"{session.Title}");
        Console.WriteLine($"{session.Timeslot}");
        Console.WriteLine($"{session.Speaker}");
        Console.WriteLine($"{session.Link}");
        Console.WriteLine("====================================");
        Console.WriteLine();
    }
}


