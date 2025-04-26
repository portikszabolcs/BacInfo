using BacInfo.Models;
using BacInfo.Services;

namespace BacInfo.Controllers
{
    public class StudentController(ResultService _resultsService)
    {
        public async Task<BacResult?> GetStudentByCode(string code)
        {
            var _bacResults = await _resultsService.GetAllResults();
            return _bacResults.Find((x) => x.CodCandidat == code);
        }
    }
}