using System;
using Map;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerOnMapShower : BaseMapShower
    {
        public override string MapShowTypeId => "player";
    }
}