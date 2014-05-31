using System;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public class GameEvent
    {
        public static Action ShipCrashed = delegate { };
    }
}

