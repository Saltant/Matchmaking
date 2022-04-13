using Saltant.Matchmaking.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saltant.Matchmaking.Interfaces
{
    public interface ITeam
    {
        Team.Owner GetOwner { get; }
        string GetPlayerName { get; }
        Rank GetRank { get; }
    }
}
