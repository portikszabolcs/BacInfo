using BacInfo.Models;
using BacInfo.Services;

namespace BacInfo.Controllers
{
    public class StatisticsController(ResultService resultsService)
    {
        private readonly ResultService _resultsService = resultsService;

        public async Task<Dictionary<string, double?>> GetGradesDistribution()
        {
            var _bacResults = await _resultsService.GetAllResults();
            Dictionary<string, double?> mediaDistribution = new();
            foreach (BacResult result in _bacResults!)
            {
                string key = result.Media.ToString("0");
                if (mediaDistribution.ContainsKey(key))
                {
                    mediaDistribution[key]++;
                }
                else
                {
                    mediaDistribution[key] = 1;
                }
            }
            mediaDistribution["<5"] = mediaDistribution["0"];
            mediaDistribution.Remove("0");
            return mediaDistribution;
        }

        public async Task<Dictionary<string, double?>> GetPassRate()
        {
            var _bacResults = await _resultsService.GetAllResults();
            Dictionary<string, double?> passRate = new();
            int reusit = _bacResults.Count((item) => item.RezultatFinal == ResultType.Reusit);
            passRate.Add("Reusit", reusit);
            passRate.Add("Respins", _bacResults.Count - reusit);
            return passRate;
        }

        public async Task<Dictionary<string, double?>> GetGradesHistogram()
        {
            var _bacResults = await _resultsService.GetAllResults();
            List<IGrouping<double, BacResult>> grades = _bacResults!.GroupBy((item) => Math.Floor(item.Media)).ToList();
            Dictionary<string, double?> histogram = new();
            grades.ForEach((group) =>
            {
                histogram.Add("≥" + group.Key.ToString(), group.Count());
            });
            histogram["≥9"] += histogram["≥10"]!.Value;
            histogram["<5"] = histogram["≥0"];
            histogram.Remove("≥0");
            histogram.Remove("≥10");
            return histogram;
        }
    }
}
