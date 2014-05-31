using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver,
    }

    public class GameController : IInitializable, ITickable
    {
        Ship _ship;
        GameState _state = GameState.WaitingToStart;
        AsteroidManager _asteroidSpawner;
        float _elapsedTime;

        public float ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public GameState State
        {
            get
            {
                return _state;
            }
        }

        public GameController(Ship ship, AsteroidManager asteroidSpawner)
        {
            _asteroidSpawner = asteroidSpawner;
            _ship = ship;
        }

        public void Initialize()
        {
            Screen.showCursor = false;
            GameEvent.ShipCrashed += OnShipCrashed;

            Debug.Log("Started Game");
        }

        public void Tick()
        {
            switch (_state)
            {
                case GameState.WaitingToStart:
                {
                    UpdateStarting();
                    break;
                }
                case GameState.Playing:
                {
                    UpdatePlaying();
                    break;
                }
                case GameState.GameOver:
                {
                    UpdateGameOver();
                    break;
                }
                default:
                {
                    Assert.That(false);
                    break;
                }
            }
        }

        void UpdateGameOver()
        {
            Assert.That(_state == GameState.GameOver);

            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        void OnShipCrashed()
        {
            Assert.That(_state == GameState.Playing);
            _state = GameState.GameOver;
            _asteroidSpawner.Stop();
        }

        void UpdatePlaying()
        {
            Assert.That(_state == GameState.Playing);
            _elapsedTime += Time.deltaTime;
        }

        void UpdateStarting()
        {
            Assert.That(_state == GameState.WaitingToStart);

            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        void StartGame()
        {
            Assert.That(_state == GameState.WaitingToStart || _state == GameState.GameOver);

            _ship.Position = Vector3.zero;
            _elapsedTime = 0;
            _asteroidSpawner.Start();
            _ship.ChangeState(EShipState.Moving);
            _state = GameState.Playing;
        }
    }
}
