using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour {

    public int currentIndex;
    public int row;
    public Dictionary<GameBoard.Directions, bool> availableDirDict = new Dictionary<GameBoard.Directions, bool>();
}
