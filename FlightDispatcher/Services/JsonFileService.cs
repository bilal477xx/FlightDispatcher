using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlightDispatcher.Services;

public static class JsonFileService<T>
{

    public static async Task SaveAsync(T data, string path)
    {
        string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
        await File.WriteAllTextAsync(path, jsonString);
    }

    public static async Task<T?> LoadAsync(string path)
    {
        string jsonString = await File.ReadAllTextAsync(path);
        return JsonConvert.DeserializeObject<T>(jsonString);
    }
    
}