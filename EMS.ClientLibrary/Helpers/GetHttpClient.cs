﻿using EMS.BaseLibrary.DTOs;

namespace EMS.ClientLibrary.Helpers
{
    public class GetHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LocalStorageService _localStorageService;
        private const string _headerKey = "Authorization";

        public GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageService = localStorageService;
        }

        public async Task<HttpClient> GetPrivateHttpClient()
        {
            var client = _httpClientFactory.CreateClient("SystemApiClient");
            var stringToken = await _localStorageService.GetToken();

            if (string.IsNullOrEmpty(stringToken))
                return client;

            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);

            if (deserializeToken is null)
                return client;

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializeToken.Token);
            return client;
        }

        public HttpClient GetPublicHttpClient()
        {
            var client = _httpClientFactory.CreateClient("SystemApiClient");
            client.DefaultRequestHeaders.Remove(_headerKey);

            return client;
        }
    }
}