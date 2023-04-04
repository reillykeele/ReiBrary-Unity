using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Util.Enums;
using Util.Singleton;

namespace Util.Systems
{
    /// <summary>
    /// Controls the flow of the game's system and the application's state.
    /// </summary>
    public class GameSystem : Singleton<GameSystem>
    {
        // [Header("Configuration")]
        // public GameConfigScriptableObject Config;

        [Header("Game State")]
        [SerializeField] private GameState _currentGameState = GameState.Playing;
        public GameState CurrentGameState => _currentGameState;
        public UnityEvent<GameState> OnGameStateChangeEvent;
        public UnityEvent OnPauseGameEvent;
        public UnityEvent OnResumeGameEvent;

        // Audio
        // TODO: Should this be elsewhere?
        [Header("Audio")]
        public AudioMixer Mixer;

        protected override void Awake()
        {
            base.Awake();

            if (FindObjectOfType<LoadingSystem>() == null)
                SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        }

        public void ChangeGameState(GameState newGameState)
        {
            if (_currentGameState == newGameState)
                return;

            switch (newGameState)
            {
                case GameState.None:
                    break;
                case GameState.Paused:
                    break;
                case GameState.Playing:
                    break;
                case GameState.Menu:
                    break;
                case GameState.Cutscene:
                    break;
            }

            _currentGameState = newGameState;
            OnGameStateChangeEvent.Invoke(newGameState);
        }

        public bool IsPlaying() => CurrentGameState == GameState.Playing;
        public bool IsPaused() => CurrentGameState == GameState.Paused;

        public void PauseGame()
        {
            if (CurrentGameState != GameState.Playing) return;

            ChangeGameState(GameState.Paused);
            OnPauseGameEvent.Invoke();

            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            if (CurrentGameState != GameState.Paused) return;

            ChangeGameState(GameState.Playing);
            OnResumeGameEvent.Invoke();

            Time.timeScale = 1;
        }

        public void TogglePaused() { if (IsPlaying()) PauseGame(); else ResumeGame(); }

        public void Quit()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit(0);
            #endif
        }
    }
}
