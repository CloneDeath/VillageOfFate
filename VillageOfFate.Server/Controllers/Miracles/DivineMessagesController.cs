using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Server.Exceptions.NotFound;
using VillageOfFate.Services.BIServices;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Server.Controllers.Miracles;

[Authorize]
[ApiController]
[Route("Miracles/[controller]")]
public class DivineMessagesController(
	UserService users,
	EventsService events,
	ItemService items,
	VillagerService villagers
) : ControllerBase {
	[HttpPost]
	public async Task PostMessage([FromBody] string message) {
		var user = await users.GetUserAsync();
		if (!user.BibleId.HasValue) throw new NotFoundException("You must have a Bible to post a message.");

		var bible = await items.GetWithLocationAsync(user.BibleId.Value);
		var location = bible.Sector ?? bible.Villager?.Sector;
		if (location == null) throw new NotFoundException("Your bible could not be found!");

		var page = await items.CreateBiblePageAsync(message, bible);

		var witnesses = await villagers.GetVillagersInSectorAsync(location.Id);
		await events.AddAsync(bible, location, witnesses, "The Bible begins to glow");

		var owner = bible.Villager;
		if (owner == null) return;

		await events.AddAsync(owner, location, [],
			$"You feel that the God of Fate has sent a new Divine Message to your Bible. You feel compelled to read it aloud! (new page Item.Id: {page.Id})");

		await villagers.QueueTriggerReactionsAsync(owner, bible, "Divine Message");
	}
}