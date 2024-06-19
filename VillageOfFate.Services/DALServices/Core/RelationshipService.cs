using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices.Core;

public class RelationshipService(DataContext context) {
	public async Task AddRelationAsync(VillagerDto villager, VillagerDto relation, string summary) {
		await context.Relationships.AddAsync(new RelationshipDto {
			Villager = villager,
			Relation = relation,
			Summary = summary
		});
		await context.SaveChangesAsync();
	}

	public async Task<IEnumerable<RelationshipDto>> GetAsync(VillagerDto villager) {
		return await context.Relationships
							.Include(r => r.Villager)
							.Include(r => r.Relation)
							.Where(r => r.VillagerId == villager.Id).ToListAsync();
	}
}