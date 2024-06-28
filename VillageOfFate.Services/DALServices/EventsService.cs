using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class EventsService(DataContext context, TimeService timeService) {
	protected IQueryable<EventDto> Events => context.Events
													.Include(e => e.Actor)
													.Include(e => e.Witnesses)
													.Include(e => e.Sector);

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
		var worldTime = await timeService.GetAsync(TimeLabel.World);
		var time = eventTime ?? worldTime;
		var mostRecentEvent = context.Events.OrderBy(p => p.Order).FirstOrDefaultAsync(e => e.Time == time);
		var order = mostRecentEvent.Result != null ? mostRecentEvent.Result.Order + 1 : 0;
		var eventEntity = await context.Events.AddAsync(new EventDto {
			Time = time,
			Actor = actor,
			Sector = sector,
			Description = description,
			Order = order
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

	public async Task<IEnumerable<EventDto>> GetVillagerEvents(Guid id) {
		return await Events.Where(e => e.ActorId == id || e.Witnesses.Any(w => w.Id == id))
						   .OrderByDescending(e => e.Time)
						   .ThenBy(e => e.Order)
						   .ToListAsync();
	}

	public async Task<IEnumerable<EventDto>> GetSectorEventsAsync(Guid sectorId) {
		return await Events.Where(e => e.SectorId == sectorId)
						   .OrderByDescending(e => e.Time)
						   .ThenBy(e => e.Order)
						   .ToListAsync();
	}
}