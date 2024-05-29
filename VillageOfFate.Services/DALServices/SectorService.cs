using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.WebModels;

namespace VillageOfFate.Services.DALServices;

public class SectorService(DataContext context) {
	public async Task<bool> SectorExists(Position position) {
		return await context.Sectors.AnyAsync(s => s.X == position.X && s.Y == position.Y);
	}

	public async Task<SectorDto> GetSector(Position position, Action<SectorDto>? sectorConstructor = null) {
		var sector = await context.Sectors.FirstOrDefaultAsync(s => s.X == position.X && s.Y == position.Y);
		if (sector != null) {
			return sector;
		}

		var result = await context.Sectors.AddAsync(new SectorDto {
			X = position.X,
			Y = position.Y
		});
		await context.SaveChangesAsync();
		sectorConstructor?.Invoke(result.Entity);
		return result.Entity;
	}
}