using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    [SerializeField]
    private Camera mainCam;

    struct Slot
    {
        public bool isFilled;
        public int row;
        public Dictionary<Directions, bool> moveDict;
        public GameObject indicator;
    }

    private Slot[] boardSlots = new Slot[15];

    public GameObject[] slotIndicators = new GameObject[15];
    public Peg[] pegs = new Peg[14];

    private Peg currentPeg;

    public enum Directions
    {
        UP_RIGHT = 1,
        RIGHT = 2,
        DOWN_RIGHT = 3,
        DOWN_LEFT = 4,
        LEFT = 5,
        UP_LEFT = 6,
        NONE = 7,
    }

    private int emptySlotIndex = 0;

    void Start ()
    {

        int rowCounter = 1;

        for (int i = 0; i < boardSlots.Length; ++i)
        {
            boardSlots[i].isFilled = true;

            if (i == emptySlotIndex)
            {
                boardSlots[i].isFilled = false;
            }

            if (i == 1 || i == 3 || i == 6 || i == 10)
            {
                rowCounter++;
            }
            boardSlots[i].row = rowCounter;
            boardSlots[i].moveDict = new Dictionary<Directions, bool>();

            boardSlots[i].moveDict.Add(Directions.UP_RIGHT, false);
            boardSlots[i].moveDict.Add(Directions.RIGHT, false);
            boardSlots[i].moveDict.Add(Directions.DOWN_RIGHT, false);
            boardSlots[i].moveDict.Add(Directions.DOWN_LEFT, false);
            boardSlots[i].moveDict.Add(Directions.LEFT, false);
            boardSlots[i].moveDict.Add(Directions.UP_LEFT, false);

            if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 5)
            {
                boardSlots[i].moveDict[Directions.DOWN_LEFT] = true;
                boardSlots[i].moveDict[Directions.DOWN_RIGHT] = true;
            }
            if (i == 3 || i == 6 || i == 7 || i == 10 || i == 11 || i == 12)
            {
                boardSlots[i].moveDict[Directions.UP_RIGHT] = true;
                boardSlots[i].moveDict[Directions.RIGHT] = true;
            }
            if (i == 5 || i == 8 || i == 9 || i == 12 || i == 13 || i == 14)
            {
                boardSlots[i].moveDict[Directions.UP_LEFT] = true;
                boardSlots[i].moveDict[Directions.LEFT] = true;
            }

            //NOTE: Getting objects with tag returns them in reversed order, subtract by 14 for correct indexing
            boardSlots[i].indicator = slotIndicators[14 - i];
        }

        for (int i = 0; i < pegs.Length; ++i)
        {
            pegs[i].availableDirDict = boardSlots[i + 1].moveDict;
        }

        if (slotIndicators[slotIndicators.Length - 1] == null)
        {
            Debug.LogWarning("SlotIndicators array has not been filled in the editor.");
        }

        if (pegs[pegs.Length - 1] == null)
        {
            Debug.LogWarning("Peg array has not been filled in the editor.");
        }
        

    }

    private void DeselectAllPegs()
    {
        for (int i = 0; i < pegs.Length; ++i)
        {
            pegs[i].selected = false;
        }
    }

    private Directions GetDirection(SlotIndicator targetSlot)
    {
        //TODO: fix this garbage

        int currentPegRow = boardSlots[currentPeg.currentIndex].row;

        if (currentPeg != null)
        {
            if (currentPeg.currentIndex > targetSlot.slotIndex)
            {
                if (currentPeg.currentIndex - targetSlot.slotIndex == 2)
                {
                    return Directions.LEFT;
                }
                else if (currentPeg.currentIndex - ((2 * currentPegRow) - 1) == targetSlot.slotIndex)
                {
                    return Directions.UP_LEFT;
                }
                else if (currentPeg.currentIndex - ((currentPegRow  - 1) + currentPegRow - 2) == targetSlot.slotIndex)
                {
                    return Directions.UP_RIGHT;
                }
            }
            else
            {
                if (currentPeg.currentIndex + 2 == targetSlot.slotIndex)
                {
                    return Directions.RIGHT;
                }
                else if(currentPeg.currentIndex + ((currentPegRow + 1) + currentPegRow + 2) == targetSlot.slotIndex)
                {
                    return Directions.DOWN_RIGHT;
                }
                else if (currentPeg.currentIndex + ((2 * currentPegRow) + 1) == targetSlot.slotIndex)
                {
                    return Directions.DOWN_LEFT;
                }
            }

            Debug.Log("Invalid direction");
            return Directions.NONE;
        }
        else
        {
            Debug.LogError("Peg is never assigned");
            return Directions.NONE;
        }
            
    }

    public void UpdatePegDirections()
    {
        //TODO: implement checking for neighbors, and updating directional options
    }

    private void MovePeg(int slotIndex, Directions dir)
    {
        Debug.Log(slotIndex);
        Debug.Log(dir);
        bool canMoveInDir;

        if (currentPeg != null)
        {
            if (currentPeg.availableDirDict.TryGetValue(dir, out canMoveInDir))
            {
                if (canMoveInDir)
                {
                    if (!boardSlots[slotIndex].isFilled)
                    {
                        currentPeg.transform.position = slotIndicators[slotIndex].transform.position;
                        boardSlots[currentPeg.currentIndex].isFilled = false;
                        boardSlots[slotIndex].isFilled = true;
                        currentPeg.currentIndex = slotIndex;
                        currentPeg.availableDirDict = boardSlots[slotIndex].moveDict;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Move dictionary did not contain key for given direction.");
            }
        }
    }

	void Update ()
    {
		if(Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (currentPeg == null)
            {
                if (hit.transform != null && hit.transform.tag == "Peg")
                {
                    currentPeg = hit.transform.GetComponent<Peg>();
                    hit.transform.SendMessage("SelectThisPeg");
                }
            }
            else
            {
                if (hit.transform != null && hit.transform.tag == "Slot")
                {
                    SlotIndicator tempIndicator = hit.transform.GetComponent<SlotIndicator>();
                    Slot s = boardSlots[tempIndicator.slotIndex];
                    if (!s.isFilled)
                    {
                        MovePeg(tempIndicator.slotIndex, GetDirection(tempIndicator));
                    }
                }
                currentPeg = null;
                DeselectAllPegs();
            }
        }
	}
}
