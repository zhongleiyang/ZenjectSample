using System;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public class ShipStateFactory
    {
        private IFactory<ShipState>[] _factories;

        public ShipStateFactory(DiContainer container)
        {
            _factories = new IFactory<ShipState>[(int) EShipState.Count]
            {
                new Factory<ShipState, ShipStateMoving>(container),
                new Factory<ShipState, ShipStateDead>(container),
                new Factory<ShipState, ShipStateWaitingToStart>(container),
            };
        }

        public ShipState Create(EShipState state, params object[] constructorArgs)
        {
            return _factories[(int)state].Create(constructorArgs);
        }
    }
}
