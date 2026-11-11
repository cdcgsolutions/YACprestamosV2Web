using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace WEB1.Module.CLIENT.Services
{
    public interface IUserPreferenceService
    {
        Task TrackPageClickAsync(string pageRoute);
    }

    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly IJSRuntime _js;
        private const string PageClicksKey = "pageClicks";

        public UserPreferenceService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task TrackPageClickAsync(string pageRoute)
        {
            var jsonData = await _js.InvokeAsync<string>("localStorage.getItem", PageClicksKey);
            var pageClicks = string.IsNullOrEmpty(jsonData)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(jsonData) ?? new Dictionary<string, int>();

            if (pageClicks.ContainsKey(pageRoute))
            {
                pageClicks[pageRoute]++;
            }
            else
            {
                pageClicks[pageRoute] = 1;
            }

            jsonData = JsonSerializer.Serialize(pageClicks);
            await _js.InvokeVoidAsync("localStorage.setItem", PageClicksKey, jsonData);
        }
    }
}
