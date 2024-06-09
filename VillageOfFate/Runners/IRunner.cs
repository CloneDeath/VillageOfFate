using System.Threading;
using System.Threading.Tasks;

namespace VillageOfFate.Runners;

public interface IRunner {
	public Task RunAsync(CancellationToken cancellationToken);
}