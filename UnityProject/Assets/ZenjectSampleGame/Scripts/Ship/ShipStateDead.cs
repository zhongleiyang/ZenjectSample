using System;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;
using Random=UnityEngine.Random;

namespace ModestTree.Asteroids
{
    public class ShipStateDead : ShipState
    {
        Settings _settings;
        float _elapsedTime;
        GameObjectInstantiator _gameObjectCreator;
        GameObject _shipBroken;
        GameObject _shipExplosion;

        public ShipStateDead(Settings settings, Ship ship, GameObjectInstantiator gameObjectCreator)
            : base(ship)
        {
            _settings = settings;
            _gameObjectCreator = gameObjectCreator;
        }

        public override void Start()
        {
            _ship.MeshRenderer.enabled = false;
            _ship.ParticleEmitter.emit = false;
            _ship.AudioSource.Play();

            _shipExplosion = _gameObjectCreator.Instantiate(_settings.explosionTemplate);
            _shipExplosion.transform.position = _ship.Position;

            _shipBroken = _gameObjectCreator.Instantiate(_settings.brokenTemplate);
            _shipBroken.transform.position = _ship.Position;
            _shipBroken.transform.rotation = _ship.Rotation;

            foreach (var rigidBody in _shipBroken.GetComponentsInChildren<Rigidbody>())
            {
                var randomTheta = Random.Range(0, Mathf.PI * 2.0f);
                var randomDir = new Vector3(Mathf.Cos(randomTheta), Mathf.Sin(randomTheta), 0);
                rigidBody.AddForce(randomDir * _settings.explosionForce);
            }

            GameEvent.ShipCrashed();
        }

        public override void Stop()
        {
            _ship.MeshRenderer.enabled = true;
            _ship.ParticleEmitter.emit = true;

            GameObject.Destroy(_shipExplosion);
            GameObject.Destroy(_shipBroken);
        }

        public override void Update()
        {
        }

        [Serializable]
        public class Settings
        {
            public GameObject brokenTemplate;
            public GameObject explosionTemplate;
            public float explosionForce;
        }
    }
}
