using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.WebModels;

namespace VillageOfFate.Services.DALServices;

public class SectorService(DataContext context, ItemService items) {
	public async Task<bool> SectorExistsAsync(Position position) {
		return await context.Sectors.AnyAsync(s => s.X == position.X && s.Y == position.Y);
	}

	public async Task<SectorDto> GetOrCreateSectorAsync(
		Position position,
		Action<SectorDto>? sectorConstructor = null
	) {
		var sector = await context.Sectors.FirstOrDefaultAsync(s => s.X == position.X && s.Y == position.Y);
		if (sector != null) {
			return sector;
		}

		var result = await context.Sectors.AddAsync(new SectorDto {
			X = position.X,
			Y = position.Y,
			Image = new ImageDto()
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
			dbItem.VillagerId = null;
			dbItem.SectorId = sector.Id;
			await context.SaveChangesAsync();
		}
	}

	public async Task<IEnumerable<SectorDto>> GetAllAsync() => await context.Sectors.ToListAsync();

	public Task<SectorDto> GetAsync(Guid id) {
		return context.Sectors.Include(s => s.Items).FirstAsync(s => s.Id == id);
	}

	public async Task<IEnumerable<SectorDto>> GetSectorsWithoutImagesAsync() {
		return await context.Sectors.Where(s => s.Image.Base64Image == null).ToListAsync();
	}

	public async Task<SectorDto?> TryGetAsync(Position position) {
		return await context.Sectors.FirstOrDefaultAsync(s => s.X == position.X && s.Y == position.Y);
	}
}