using System;
using System.Collections.Generic;
using System.Text;

namespace Saltant.Matchmaking.Interfaces
{
    public interface IGameMatch
    {
        Guid GetMatchId { get; }
    }
}
