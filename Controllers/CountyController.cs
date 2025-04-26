using BacInfo.Models;
using System.Collections.Immutable;
using BacInfo.Services;

namespace BacInfo.Controllers
{
    public class CountyController(ResultService resultsService)
    {
        private readonly ResultService _resultsService = resultsService;

        public async Task<ImmutableSortedSet<string>> GetCountyNames()
        {
            var _bacResults = await _resultsService.GetAllResults();
            ImmutableSortedSet<string> counties = _bacResults
                .DistinctBy(x => x.JudetNume).Select((item) => item.JudetNume)
                .ToImmutableSortedSet();
            return counties;
        }

        public async Task<List<double?>> GetCountyAverages()
        {
            var _bacResults = await _resultsService.GetAllResults();
            IEnumerable<IGrouping<string, BacResult>> groups = _bacResults.GroupBy((item) => item.JudetNume).OrderBy((item) => item.Key);

            return groups.Select((group) => (double?)group.Sum((item) => item.Media) / group.Count()).ToList();
        }
    }
}
