using Microsoft.AspNetCore.Mvc;

namespace EstateApp.MyLogging
{
    public class DemoController : Controller
    {
        // 1. Loosely coupled dependency injection
        private readonly IMyLogger _myLogger;

        // Correctly assigning the constructor parameter to the class field
        public DemoController(IMyLogger myLogger)
        {
            _myLogger = myLogger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            _myLogger.Log("Index method started");
            return Ok();
        }
    }
}
