using System.Threading;
using System.Threading.Tasks;

namespace VillageOfFate;

public interface IRunner {
	public Task RunAsync(CancellationToken cancellationToken);
}