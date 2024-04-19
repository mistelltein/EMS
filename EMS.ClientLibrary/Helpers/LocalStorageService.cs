using Blazored.LocalStorage;

namespace EMS.ClientLibrary.Helpers
{
    public class LocalStorageService
    {
        private readonly ILocalStorageService _localStorageService;
        private const string _storageKey = "authentication-token";

        public LocalStorageService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<string> GetToken() =>
            await _localStorageService.GetItemAsStringAsync(_storageKey);

        public async Task SetToken(string item) =>
            await _localStorageService.SetItemAsStringAsync(_storageKey, item);

        public async Task RemoveToken() =>
            await _localStorageService.RemoveItemAsync(_storageKey);
    }
}