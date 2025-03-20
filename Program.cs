// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Text.Json;

// We call 3 methods in this code: 
// One to get the REST data, 
var content = await GetContentFromUrlAsync("https://bigconference.azurewebsites.net/sessions");
// one to parse the JSON data, 
var sessions = ParseSessions(content);
// and one to print the data.
PrintSessions(sessions);


// This method gets the data from the REST API 
// You can get see the data by navigating to the link above
// The key is a secret key that is required to access the data but not used in this example
async Task<string> GetContentFromUrlAsync(string url)
{
    using var client = new HttpClient();
    string? key = "sdssdsdsd";
    var response = await client.GetAsync(url);
    client.DefaultRequestHeaders.Add("authorization", key);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}

// This method parses the JSON data and returns a list of  ** ConferenceSession ** objects. 
// See the classlib.cs file for the definition of the  **  ConferenceSession ** class
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

// Finally, this method prints the data to the console
// The data is a list of ** ConferenceSession ** objects which is passed to this method
// See the classlib.cs file for the definition of the  **  ConferenceSession ** class
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


