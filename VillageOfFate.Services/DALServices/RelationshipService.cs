using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class RelationshipService(DataContext context) {
	public async Task AddRelationAsync(VillagerDto villager, VillagerDto relation, string summary) {
		await context.Relationships.AddAsync(new RelationshipDto {
			VillagerId = villager.Id,
			RelationId = relation.Id,
			Summary = summary
		});
		await context.SaveChangesAsync();
	}
}