using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinLosePanel : MonoBehaviour {

    private Text title;
    private Text description;

    private Button playAgainButton;

    private Vector3 startPos;
    private float dt = 0.0f;

    private void Awake()
    {
        title = this.gameObject.transform.GetChild(0).GetComponent<Text>();
        description = this.gameObject.transform.GetChild(1).GetComponent<Text>();
        playAgainButton = this.gameObject.transform.GetChild(2).GetComponent<Button>();

        playAgainButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });
    }
    void Start ()
    {
        startPos = transform.position;
        transform.position += Vector3.left * transform.position.x * 2;
	}
	
	void Update ()
    {
        if (dt < 1.0f)
        {
            if (GameStateManager._gameState == GameStateManager.GameState.WIN)
            {
                dt += Time.deltaTime / 4;
                transform.position = Vector3.Lerp(transform.position, startPos, dt);
                title.text = "You Won!";
                description.text = "Congratulations! Can you do it again?";
            }
            if (GameStateManager._gameState == GameStateManager.GameState.LOSE)
            {
                dt += Time.deltaTime / 4;
                transform.position = Vector3.Lerp(transform.position, startPos, dt);
                title.text = "You Lost";
                description.text = "Try again for a better score?";
            }
        }
	}
}
