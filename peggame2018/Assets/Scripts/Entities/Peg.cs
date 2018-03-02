using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour {

    [Header("Position")]
    public int currentIndex;
    public int row;
    public Dictionary<GameBoard.Directions, bool> availableDirDict = new Dictionary<GameBoard.Directions, bool>();

    public bool isMoving = false;
    public Vector3 targetPosition;
    private float dt = 0.0f;

    private MeshRenderer meshRend;

    [Header("Visuals")]
    [SerializeField]
    private Material defaultMat;
    [SerializeField]
    private Material selectedMat;

    void Awake()
    {
        meshRend = gameObject.GetComponent<MeshRenderer>();
    }

    public void Select()
    {
        meshRend.sharedMaterial = selectedMat;
    }

    public void Deselect()
    {
        meshRend.sharedMaterial = defaultMat;
    }

    public void SetMoving(Vector3 target, bool move)
    {
        isMoving = move;
        targetPosition = target;
    }

    //NOTE: I'm leaving this TODO in to demonstrate how I typically handle future optimizations 
    //      since I'm running at about 600FPS on my laptop without vsync.

    //TODO: (BEN) implement elsewhere, ideally one Update call handles Peg movement
    private void Update()
    {
        if(isMoving)
        {
            transform.position = Vector3.Slerp(transform.position, targetPosition, dt);
            dt += Time.deltaTime;
            if (dt > 1.0f)
            {
                isMoving = false;
                dt = 0;
            }
        }
    }
}
