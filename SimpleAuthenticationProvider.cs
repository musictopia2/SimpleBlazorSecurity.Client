using BasicBlazorLibrary.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
namespace SimpleBlazorSecurity.Client
{
    public class SimpleAuthenticationProvider : AuthenticationStateProvider
    {
        private readonly IFinishAuthentication? _finish;
        private readonly ILocalKeyClass _key;
        private readonly IJSRuntime _js;
        private readonly HttpClient _http;
        public SimpleAuthenticationProvider(ILocalKeyClass key, IJSRuntime js, HttpClient http)
        {
            _key = key;
            _js = js;
            _http = http;
        }

        public SimpleAuthenticationProvider(ILocalKeyClass key, IJSRuntime js, HttpClient http, IFinishAuthentication finish)
        {
            _key = key;
            _js = js;
            _http = http;
            _finish = finish;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string key = _key.KeyName;
            string authToken = await _js.StorageGetStringAsync(key);

            var identity = new ClaimsIdentity();
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    if (_finish != null)
                    {
                        await _finish.AfterAuthenticationAsync();
                    }
                }
                catch (Exception)
                {
                    await _js.StorageRemoveItemAsync(key);
                    identity = new ClaimsIdentity();
                }
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes); //this can use the new json serializers this time.  hopefully i won't regret this.
            //if i regret this, then would switch to using the newton one.
            var claims = keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
            return claims;
        }
    }
}