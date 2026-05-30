using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediSenior.Services;

public class FirestoreService
{
    private readonly HttpClient _httpClient;
    private readonly SessionService _session;

    public FirestoreService(HttpClient httpClient, SessionService session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task SetDocumentAsync(string collection, string documentId, Dictionary<string, object?> data)
    {
        var url = $"{FirebaseConfig.FirestoreBaseUrl}/{collection}/{documentId}";
        using var request = new HttpRequestMessage(HttpMethod.Patch, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.IdToken);
        request.Content = JsonContent.Create(ToFirestoreDocument(data));

        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Błąd zapisu Firestore: {body}");
    }

    public async Task<string> AddDocumentAsync(string collection, Dictionary<string, object?> data)
    {
        var url = $"{FirebaseConfig.FirestoreBaseUrl}/{collection}";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.IdToken);
        request.Content = JsonContent.Create(ToFirestoreDocument(data));

        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Błąd dodawania Firestore: {body}");

        using var doc = JsonDocument.Parse(body);
        var name = doc.RootElement.GetProperty("name").GetString() ?? "";
        return name.Split('/').Last();
    }

    public async Task<Dictionary<string, object?>?> GetDocumentAsync(string collection, string documentId)
    {
        var url = $"{FirebaseConfig.FirestoreBaseUrl}/{collection}/{documentId}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.IdToken);

        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Błąd odczytu Firestore: {body}");

        using var doc = JsonDocument.Parse(body);
        return FromFirestoreDocument(doc.RootElement);
    }

    public async Task<List<(string Id, Dictionary<string, object?> Data)>> QueryEqualAsync(
        string collection,
        string field,
        string value)
    {
        var url = $"{FirebaseConfig.FirestoreBaseUrl}:runQuery";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.IdToken);

        request.Content = JsonContent.Create(new
        {
            structuredQuery = new
            {
                from = new[] { new { collectionId = collection } },
                where = new
                {
                    fieldFilter = new
                    {
                        field = new { fieldPath = field },
                        op = "EQUAL",
                        value = new { stringValue = value }
                    }
                }
            }
        });

        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Błąd zapytania Firestore: {body}");

        var result = new List<(string Id, Dictionary<string, object?> Data)>();

        using var doc = JsonDocument.Parse(body);
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            if (!item.TryGetProperty("document", out var document))
                continue;

            var name = document.GetProperty("name").GetString() ?? "";
            var id = name.Split('/').Last();
            var data = FromFirestoreDocument(document);

            result.Add((id, data));
        }

        return result;
    }

    private static object ToFirestoreDocument(Dictionary<string, object?> data)
    {
        var fields = new Dictionary<string, object>();

        foreach (var item in data)
        {
            fields[item.Key] = ToFirestoreValue(item.Value);
        }

        return new { fields };
    }

    private static object ToFirestoreValue(object? value)
    {
        if (value == null)
            return new { nullValue = "NULL_VALUE" };

        if (value is string s)
            return new { stringValue = s };

        if (value is bool b)
            return new { booleanValue = b };

        if (value is int i)
            return new { integerValue = i.ToString() };

        if (value is long l)
            return new { integerValue = l.ToString() };

        if (value is double d)
            return new { doubleValue = d };

        if (value is IEnumerable<string> stringList)
        {
            return new
            {
                arrayValue = new
                {
                    values = stringList.Select(x => new { stringValue = x }).ToArray()
                }
            };
        }

        throw new NotSupportedException($"Nieobsługiwany typ Firestore: {value.GetType().Name}");
    }

    private static Dictionary<string, object?> FromFirestoreDocument(JsonElement document)
    {
        var result = new Dictionary<string, object?>();

        if (!document.TryGetProperty("fields", out var fields))
            return result;

        foreach (var field in fields.EnumerateObject())
        {
            result[field.Name] = FromFirestoreValue(field.Value);
        }

        return result;
    }

    public async Task UpdateDocumentAsync(string collection, string documentId, Dictionary<string, object?> data)
    {
        await SetDocumentAsync(collection, documentId, data);
    }

    private static object? FromFirestoreValue(JsonElement value)
    {
        if (value.TryGetProperty("stringValue", out var s))
            return s.GetString();

        if (value.TryGetProperty("booleanValue", out var b))
            return b.GetBoolean();

        if (value.TryGetProperty("integerValue", out var i))
            return int.Parse(i.GetString() ?? "0");

        if (value.TryGetProperty("doubleValue", out var d))
            return d.GetDouble();

        if (value.TryGetProperty("arrayValue", out var arr))
        {
            var list = new List<string>();

            if (arr.TryGetProperty("values", out var values))
            {
                foreach (var item in values.EnumerateArray())
                {
                    if (item.TryGetProperty("stringValue", out var itemString))
                        list.Add(itemString.GetString() ?? "");
                }
            }

            return list;
        }

        return null;
    }
}
