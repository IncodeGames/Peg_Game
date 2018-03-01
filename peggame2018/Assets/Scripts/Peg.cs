using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour {

    public int currentIndex;

    public bool selected;

    private void SelectThisPeg()
    {
        selected = true;
    }
}
