using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.DAL;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
	public DbSet<Time> Time { get; set; } = null!;
}