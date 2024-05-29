using AsyncUtilities;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.WebModels;

namespace VillageOfFate.Services.DALServices;

public class SectorService(DataContext context, ItemService items) {
	public async Task<bool> SectorExistsAsync(Position position) {
		return await context.Sectors.AnyAsync(s => s.X == position.X && s.Y == position.Y);
	}

	public async Task<SectorDto>
		GetOrCreateSectorAsync(Position position, Action<SectorDto>? sectorConstructor = null) {
		var sector = await context.Sectors.FirstOrDefaultAsync(s => s.X == position.X && s.Y == position.Y);
		if (sector != null) {
			return sector;
		}

		var result = await context.Sectors.AddAsync(new SectorDto {
			X = position.X,
			Y = position.Y
		});
		await context.SaveChangesAsync();
		if (sectorConstructor == null) return result.Entity;

		sectorConstructor.Invoke(result.Entity);
		context.Sectors.Update(result.Entity);
		await context.SaveChangesAsync();
		return result.Entity;
	}

	public async Task AddItemRangeToSectorAsync(SectorDto sector, IEnumerable<ItemDto> itemsToAdd) {
		var dbItems = await itemsToAdd.Select(items.EnsureExistsAsync);
		await context.SectorItems.AddRangeAsync(dbItems.Select(i => new SectorItemDto {
			ItemId = i.Id,
			SectorId = sector.Id
		}));
		await context.SaveChangesAsync();
	}
}