using System;
using ModestTree;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public class Asteroid : IDisposable
    {
        LevelHelper _level;
        Settings _settings;
        AsteroidHooks _hooks;
        bool _hasDisposed;

        public Asteroid(
            AsteroidHooks hooks, LevelHelper level, Settings settings)
        {
            _hooks = hooks;
            _settings = settings;
            _level = level;
        }

        public Vector3 Position
        {
            get
            {
                return _hooks.transform.position;
            }
            set
            {
                _hooks.transform.position = value;
            }
        }

        public float Mass
        {
            get
            {
                return _hooks.Rigidbody.mass;
            }
            set
            {
                _hooks.Rigidbody.mass = value;
            }
        }

        public float Scale
        {
            get
            {
                var scale = _hooks.transform.localScale;
                // We assume scale is uniform
                Assert.That(scale[0] == scale[1] && scale[1] == scale[2]);

                return scale[0];
            }
            set
            {
                _hooks.transform.localScale = new Vector3(value, value, value);
                _hooks.Rigidbody.mass = value;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return _hooks.Rigidbody.velocity;
            }
            set
            {
                _hooks.Rigidbody.velocity = value;
            }
        }

        public void Update()
        {
            LimitSpeed();
            CheckForTeleport();
        }

        public void Dispose()
        {
            Assert.That(!_hasDisposed);
            _hasDisposed = true;
            GameObject.Destroy(_hooks.gameObject);
            _hooks = null;
        }

        void LimitSpeed()
        {
            var speed = _hooks.Rigidbody.velocity.magnitude;

            if (speed > _settings.maxSpeed)
            {
                var dir = _hooks.Rigidbody.velocity / speed;
                _hooks.Rigidbody.velocity = dir * _settings.maxSpeed;
            }
        }

        void CheckForTeleport()
        {
            if (Position.x > _level.Right + Scale && IsMovingInDirection(Vector3.right))
            {
                _hooks.transform.SetX(_level.Left - Scale);
            }
            else if (Position.x < _level.Left - Scale && IsMovingInDirection(-Vector3.right))
            {
                _hooks.transform.SetX(_level.Right + Scale);
            }
            else if (Position.y < _level.Bottom - Scale && IsMovingInDirection(-Vector3.up))
            {
                _hooks.transform.SetY(_level.Top + Scale);
            }
            else if (Position.y > _level.Top + Scale && IsMovingInDirection(Vector3.up))
            {
                _hooks.transform.SetY(_level.Bottom - Scale);
            }
            _hooks.transform.RotateAround(_hooks.transform.position, Vector3.up, 30 * Time.deltaTime);
        }

        bool IsMovingInDirection(Vector3 dir)
        {
            return Vector3.Dot(dir, _hooks.Rigidbody.velocity) > 0;
        }

        [Serializable]
        public class Settings
        {
            public float massScaleFactor;
            public float maxSpeed;
        }
    }
}
