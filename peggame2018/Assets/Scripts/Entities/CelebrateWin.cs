using UnityEngine;

public class CelebrateWin : MonoBehaviour {

    private ParticleSystem partSys;

    private void Awake()
    {
        partSys = this.gameObject.GetComponent<ParticleSystem>();
    }

    void Update ()
    {
		if (GameStateManager._gameState == GameStateManager.GameState.WIN && !partSys.isPlaying)
        {
            partSys.Play();
        }
	}
}
