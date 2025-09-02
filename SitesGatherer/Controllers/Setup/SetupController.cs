using Microsoft.AspNetCore.Mvc;
using SitesGatherer.Controllers.Setup.models;
using SitesGatherer.Sevices.LeadsService;
using SitesGatherer.Sevices.PagesHandler;
using SitesGatherer.Sevices.Settings;

namespace SitesGatherer.Controllers.Setup
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private IPagesHandler pagesHandler;
        private ILeadsGenerator leadsGenerator;
        private ISettingsService settingsService;
        public SetupController(IPagesHandler pagesHandler, ILeadsGenerator leadsGenerator, ISettingsService settingsService)
        {
            this.pagesHandler = pagesHandler;
            this.leadsGenerator = leadsGenerator;
            this.settingsService = settingsService;
        }

        [Route("Start")]
        [HttpGet]
        public async Task<IActionResult> Start()
        {
            Task.Run(() =>
            {
                pagesHandler.Start();
            });

            return Ok("Request received.");
        }

        [Route("GetLeads")]
        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            var res = this.leadsGenerator.GetLeads();
            return Ok(res);
        }

        [Route("SetConfig")]
        [HttpPost]
        public async Task<IActionResult> SetConfig([FromBody]ConfigModel model)
        {
            this.settingsService.SetConfigs(model);
            return Ok("Request received.");
        }


    }

}