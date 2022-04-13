using Saltant.Matchmaking.Interfaces;
using Saltant.Matchmaking.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saltant.Matchmaking
{
    partial class Matchmaker
    {
        internal class Events
        {
            public event Action<GameType, string, string, Rank> CreateTeam;
            public event Action<GameType> TryCreateMatch;
            public event Action<IGameMatch> MatchCreated;
            public event Action<IGameMatch> RemoveMatch;

            public void InvokeEvent(GameType gameType, EventType eventType, object obj1 = null, object obj2 = null, object obj3 = null)
            {
                switch (eventType)
                {
                    case EventType.CreateTeam:
                        CreateTeam.Invoke(gameType, (string)obj1, (string)obj2, (Rank)obj3);
                        break;
                    case EventType.TryCreateMatch:
                        TryCreateMatch.Invoke(gameType);
                        break;
                    case EventType.MatchCreated:
                        switch (gameType)
                        {
                            case GameType.Thunderdome:
                                MatchCreated.Invoke((ThunderdomeGameMatch)obj1);
                                break;
                            case GameType.Team5x5:
                                break;
                            case GameType.BattleRoyale:
                                break;
                            case GameType.Deathmatch:
                                break;
                        }

                        break;
                    case EventType.RemoveMatch:
                        RemoveMatch.Invoke((IGameMatch)obj1);
                        break;
                }
            }

            public enum EventType
            {
                CreateTeam,
                TryCreateMatch,
                MatchCreated,
                RemoveMatch
            }
        }
    }
}
