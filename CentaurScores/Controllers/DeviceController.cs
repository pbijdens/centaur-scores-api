using CentaurScores.Model;
using CentaurScores.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentaurScores.Controllers
{
    /// <summary>
    /// Methods specifically for the mobile devices.
    /// </summary>
    /// <remarks>Constructor</remarks>
    [ApiController]
    [Route("/devices")]
    public class DeviceController(IMatchRepository matchRepository)
    {
        /// <summary>
        /// Returns true if a model change was pushed remotely that impacts this device.
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <returns>true if a model sync is strongly suggested, false otherwise.</returns>
        [HttpGet("{deviceId}/sync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> GetDeviceNeedsSync([FromRoute] string deviceId)
        {
            return await matchRepository.CheckDeviceSynchronization(deviceId);
        }

        /// <summary>
        /// Use to clear the flag after the device synchronization was done.
        /// </summary>
        /// <param name="deviceId">The Device ID</param>
        /// <returns>Irrelevant</returns>
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
