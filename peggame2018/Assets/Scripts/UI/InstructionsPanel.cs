using UnityEngine;
using UnityEngine.UI;

public class InstructionsPanel : MonoBehaviour {

    private GameObject instructionsPanel;
    private RectTransform panelRectTrans;
    
    [SerializeField]
    private Button startButton;

    private Vector3 offscreen;
    private float dt = 0.0f;

    private void Awake()
    {
        instructionsPanel = this.gameObject;
        panelRectTrans = instructionsPanel.GetComponent<RectTransform>();
    }

    void Start ()
    {
        if (PlayerPrefs.GetInt("HasPlayed") == 1)
        {
            instructionsPanel.SetActive(false);
        }

		if (startButton == null)
        {
            Debug.LogWarning("Start button in instructions panel is null.");
            return;
        }

        startButton.onClick.AddListener(() => { GameStateManager._gameState = GameStateManager.GameState.RUNNING; PlayerPrefs.SetInt("HasPlayed", 1); });

        offscreen = new Vector3(-panelRectTrans.rect.width * 2, transform.position.y, 0);
    }

    void Update()
    {
        if (GameStateManager._gameState == GameStateManager.GameState.RUNNING)
        {
            if (dt < 1.0f)
            {
                dt += Time.deltaTime / 4;
                panelRectTrans.position = Vector3.Lerp(panelRectTrans.position, offscreen, dt);
            }
        }
    }
}
