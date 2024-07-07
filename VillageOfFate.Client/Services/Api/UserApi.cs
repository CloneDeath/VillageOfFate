using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class UserApi(ApiClient client) {
	public async Task LoginAsync() => await client.GetAsync("/User/Login");
	public async Task<WebUser> GetCurrentUserAsync() => await client.GetAsync<WebUser>("/User/Me");
}