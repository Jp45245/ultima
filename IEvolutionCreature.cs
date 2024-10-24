using System;
using Server;

namespace Server.Interfaces
{
    public interface IEvolutionCreature
    {
        int KillPoints { get; set; }
        int Level { get; }
        int MaxKillPoints { get; }
    }
}