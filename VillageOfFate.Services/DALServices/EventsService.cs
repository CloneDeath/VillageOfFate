using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class EventsService(DataContext context, TimeService time) {
	public Task AddAsync(SectorDto sector, IEnumerable<VillagerDto> witnesses, string description,
						 DateTime? eventTime = null) =>
		AddAsync(null, sector, witnesses, description, eventTime);

	public Task AddAsync(VillagerDto actor, string description, DateTime? eventTime = null) =>
		AddAsync(actor, actor.Sector, [], description, eventTime);

	public Task AddAsync(VillagerDto actor, IEnumerable<VillagerDto> witnesses, string description,
						 DateTime? eventTime = null) =>
		AddAsync(actor, actor.Sector, witnesses, description, eventTime);

	public async Task AddAsync(VillagerDto? actor, SectorDto sector, IEnumerable<VillagerDto> witnesses,
							   string description, DateTime? eventTime = null) {
		var worldTime = await time.GetAsync(TimeLabel.World);
		var eventEntity = await context.Events.AddAsync(new EventDto {
			Time = eventTime ?? worldTime,
			Actor = actor,
			Sector = sector,
			Description = description
		});

		var witnessesToAdd = witnesses;
		if (actor != null) {
			witnessesToAdd = witnessesToAdd.Append(actor);
		}

		foreach (var witness in witnessesToAdd.DistinctBy(v => v.Id)) {
			await context.EventWitnesses.AddAsync(new EventWitnessDto {
				Event = eventEntity.Entity,
				Villager = witness
			});
		}

		await context.SaveChangesAsync();
	}
}