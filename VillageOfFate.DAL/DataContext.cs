using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.DAL;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
	public DbSet<Time> Time { get; set; } = null!;

	public DbSet<VillagerDto> Villagers { get; set; } = null!;
	public DbSet<ItemDto> Items { get; set; } = null!;
	public DbSet<VillagerItemDto> VillagerItems { get; set; } = null!;
}