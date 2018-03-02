using UnityEngine;

public class GameStateManager : MonoBehaviour {

    /*NOTE: In a more sophisticated game this could be
    //      a component system, singleton, finite state
    //      machine, etc., and might be handled by a
    //      custom event system.
    */

	public enum GameState
    {
        INTRO,
        RUNNING,
        WIN,
        LOSE,
    }

    public static GameState _gameState;

	void Start ()
    {
        _gameState = GameState.INTRO;

        if (PlayerPrefs.GetInt("HasPlayed") == 1)
        {
            _gameState = GameState.RUNNING;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
