using System.Threading.Tasks;

namespace VillageOfFate.Client.Services.Api;

public class UserApi(ApiClient client) {
	public async Task LoginAsync() {
		await client.GetAsync("/User/Login");
	}
}