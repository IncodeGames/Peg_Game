using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour {

    public int currentIndex;
    public Dictionary<GameBoard.Directions, bool> availableDirDict = new Dictionary<GameBoard.Directions, bool>();
        
    public bool selected;

    public void CheckNeighbors()
    {
        selected = true;
    }
}
