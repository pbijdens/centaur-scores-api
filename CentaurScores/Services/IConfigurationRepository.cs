namespace CentaurScores.Services
{
    /// <summary>
    /// Configuration repository.
    /// </summary>
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Returns the value belonging to the specified key, or null if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string?> GetValue(string key);

        /// <summary>
        /// Updates the configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> SetValue(string key, string? value);
    }
}