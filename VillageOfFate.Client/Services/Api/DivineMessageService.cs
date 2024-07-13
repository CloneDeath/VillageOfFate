using System.Threading.Tasks;

namespace VillageOfFate.Client.Services.Api;

[RegisterClientService]
public class DivineMessageService(ApiClient client) {
	public async Task SendDivineMessageAsync(string message) {
		await client.PostAsync("Miracles/DivineMessages", message);
	}
}