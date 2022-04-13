
using Saltant.Matchmaking.Interfaces;
using Saltant.Matchmaking.Models;

namespace Saltant.Matchmaking
{
    public class Team
    {
        readonly Matchmaker.GameType gameType;
        readonly string playerName, data;
        readonly Rank rank;
        public Team(Matchmaker.GameType gameType, string playerName, string data, Rank rank = Rank.Training)
        {
            this.gameType = gameType;
            this.playerName = playerName;
            this.data = data;
            this.rank = rank;
        }

        public ITeam Initialize()
        {
            return gameType switch
            {
                Matchmaker.GameType.None => throw new System.Exception("Game Type is NONE."),
                Matchmaker.GameType.Thunderdome => new ThunderdomeTeam(playerName, data, rank),
                Matchmaker.GameType.Team5x5 => throw new System.NotImplementedException(),
                Matchmaker.GameType.BattleRoyale => throw new System.NotImplementedException(),
                Matchmaker.GameType.Deathmatch => throw new System.NotImplementedException(),
                _ => null
            };
        }

        public enum Owner
        {
            None,
            Player1,
            Player2
        }
    }
}
