using CentaurScores.Model;

namespace CentaurScores.Services
{
    public interface IFinalsService
    {
        /// <summary>
        /// Creates a new finals match based on the specified template.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        Task<MatchModel> CreateFromMatch(int id, FinalMatchDefinition match);
        /// <summary>
        /// Activate next round
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task GotoNextRound(int id);


        /// <summary>
        /// Go back to the previous round
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task GotoPreviousRound(int id);

        /// <summary>
        /// Update winner
        /// </summary>
        /// <param name="id"></param>
        /// <param name="discipline"></param>
        /// <param name="bracket"></param>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        /// <returns></returns>
        Task UpdateFinalsBracketWinner(int id, string discipline, int bracket, int winner, int loser);
    }
}