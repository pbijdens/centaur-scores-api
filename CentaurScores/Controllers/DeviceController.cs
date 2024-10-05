using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    [ApiController]
    [Route("/devices")]
    public class DeviceController
    {
        private readonly IMatchRepository matchRepository;

        public DeviceController(IMatchRepository matchRepository) 
        {
            this.matchRepository = matchRepository;
        }

        /// <summary>
        /// Returns all pre-defined participants lists for this organization.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{deviceId}/sync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> GetDeviceNeedsSync([FromRoute] string deviceId)
        {
            return await matchRepository.CheckDeviceSynchronization(deviceId);
        }

        /// <summary>
        /// Returns all pre-defined participants lists for this organization.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{deviceId}/sync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> DeleteDeviceNeedsSync([FromRoute] string deviceId)
        {
            await matchRepository.ClearDeviceSynchronization(deviceId);
            return true;
        }
    }
}
