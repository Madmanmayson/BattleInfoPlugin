﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleInfoPlugin.Models;
using Livet;

namespace BattleInfoPlugin.ViewModels
{
    public class AirCombatResultViewModel : ViewModel, IAirCombatResultViewModel
    {
        public string Name { get; }
        public bool IsHappen { get; }
        public int Count { get; }
        public int LostCount { get; }
        public int RemainingCount => this.Count - this.LostCount;

        public AirCombatResultViewModel(AirCombatResult result, FleetType type)
        {
            if (type == FleetType.Friend)
            {
                this.Name = result.Name;
                this.Count = result.FriendCount;
                this.LostCount = result.FriendLostCount;
            }
            if (type == FleetType.Enemy)
            {
                this.Name = result.Name;
                this.Count = result.EnemyCount;
                this.LostCount = result.EnemyLostCount;
            }

            this.IsHappen = result.IsHappen && this.Count > 0;
        }
    }
}
