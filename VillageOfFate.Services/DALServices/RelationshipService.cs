using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class RelationshipService(DataContext context) {
	public async Task AddRelationAsync(VillagerDto villager, VillagerDto relation, string summary) {
		await context.Relationships.AddAsync(new RelationshipDto {
			Villager = villager,
			Relation = relation,
			Summary = summary
		});
		await context.SaveChangesAsync();
	}

	public IEnumerable<RelationshipDto> Get(VillagerDto villager) {
		return context.Relationships.Where(r => r.VillagerId == villager.Id);
	}
}