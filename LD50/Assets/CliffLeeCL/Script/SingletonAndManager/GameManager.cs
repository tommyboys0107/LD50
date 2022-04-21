using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CliffLeeCL
{
    /// <summary>
    /// This singleton and facade class manage main processes in the game.
    /// </summary>
    public class GameManager : SingletonMono<GameManager>
    {
        /// <summary>
        /// For count down round time.
        /// </summary>
        public Timer roundTimer;

        /// <summary>
        /// Is true when the game is over.
        /// </summary>
        bool isGameOver = false;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
            roundTimer = gameObject.AddComponent<Timer>(); 
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// This function is called after a new level was loaded.
        /// </summary>
        /// <param name="level">The index of the level that was loaded.</param>
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 0.0f;
            isGameOver = false;
            AudioManager.Instance.PlayBGM(AudioManager.AudioName.MenuBGM);
        }

        public void GameOver()
        {
            if (isGameOver)
                return;
            
            isGameOver = true;
            roundTimer.StopTimer();
            EventManager.Instance.OnGameOver();
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySound(AudioManager.AudioName.GameOver); 
        }

        public void GameStart()
        {
            Time.timeScale = 1.0f;
            roundTimer.StartTimer();
            AudioManager.Instance.PlayBGM(AudioManager.AudioName.GameBGM);
        }
    }
}
