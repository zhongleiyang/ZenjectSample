using System;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public class ShipStateFactory
    {
        Instantiator _instantiator;

        public ShipStateFactory(Instantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public ShipState Create(EShipState state, params object[] constructorArgs)
        {
            switch (state)
            {
                case EShipState.Dead:
                    return _instantiator.Instantiate<ShipStateDead>(constructorArgs);

                case EShipState.Moving:
                    return _instantiator.Instantiate<ShipStateMoving>(constructorArgs);

                case EShipState.WaitingToStart:
                    return _instantiator.Instantiate<ShipStateWaitingToStart>(constructorArgs);
            }

            Assert.That(false);
            return null;
        }
    }
}
