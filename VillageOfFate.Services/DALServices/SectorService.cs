using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices.Core;
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
		foreach (var itemDto in itemsToAdd) {
			var dbItem = await items.EnsureExistsAsync(itemDto);
			await context.SectorItems.AddAsync(new SectorItemDto {
				Item = dbItem,
				Sector = sector
			});
			await context.SaveChangesAsync();
		}
	}

	public async Task<IEnumerable<SectorDto>> GetAll() => await context.Sectors.ToListAsync();

	public Task<SectorDto> Get(Guid id) {
		return context.Sectors.Include(s => s.Items).FirstAsync(s => s.Id == id);
	}

	public IEnumerable<SectorDto> GetSectorsWithoutImages() {
		return context.Sectors.Where(s => s.ImageId == null);
	}
}