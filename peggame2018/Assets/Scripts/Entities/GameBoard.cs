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

    //NOTE: could use a bit array for smaller memory footprint, or hashtable for very large boards

    private Slot[] boardSlots = new Slot[15];

    [SerializeField]
    private Peg[] pegLayout = new Peg[15];

    private Peg currentPeg;
    private Peg jumpedPeg;

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

    private const int emptySlotIndex = 0;

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

            boardSlots[i].indicator = GameObject.Find("SlotIndicator (" + i + ")");
            if(boardSlots[i].indicator == null)
            {
                Debug.LogWarning("Slot indicator " + i + " is misnamed or missing in the hierarchy.");
            }
        }

        for (int i = 0; i < pegLayout.Length; ++i)
        {
            if (pegLayout[i] != null)
            {
                pegLayout[i].availableDirDict = boardSlots[i].moveDict;
                pegLayout[i].row = boardSlots[i].row;
            }
            else
            {
                if (i != emptySlotIndex)
                {
                    //NOTE: Could use list if wanting to check for duplicates using .Contains()
                    Debug.LogWarning("Peg array has been improperly filled in the editor.");
                }
            }
        }
    }

    private int TargetIndexFromDir(int startIndex, Directions dir)
    {
        if (startIndex < 0)
        {
            Debug.LogWarning("Cannot find target from negative index.");
            return -1;
        }

        int ro = boardSlots[startIndex].row;
        switch (dir)
        {
            case Directions.UP_RIGHT:
                return startIndex - ((2 * ro) - 3);
            case Directions.RIGHT:
                return startIndex + 2;
            case Directions.DOWN_RIGHT:
                return startIndex + ((2 * ro) + 3);
            case Directions.DOWN_LEFT:
                return startIndex + ((2 * ro) + 1);
            case Directions.LEFT:
                return startIndex - 2;
            case Directions.UP_LEFT:
                return startIndex - ((2 * ro) - 1);
            default:
                Debug.Log("No usable direction given");
                break;
        }
        return -1;
    }

    private Directions DirectionFromTarget(SlotIndicator targetSlot)
    {
        if (currentPeg != null)
        {
            int currentIndex = currentPeg.currentIndex;
            int currentRow = currentPeg.row;

            if (currentIndex > targetSlot.slotIndex)
            {
                if (currentIndex - ((2 * currentRow) - 3) == targetSlot.slotIndex)
                {
                    return Directions.UP_RIGHT;
                }
                if (currentIndex - ((2 * currentRow) - 1) == targetSlot.slotIndex)
                {
                    return Directions.UP_LEFT;
                }
                if (currentIndex - targetSlot.slotIndex == 2)
                {
                    return Directions.LEFT;
                }
            }
            else
            {
                if (currentIndex + ((2 * currentRow) + 3) == targetSlot.slotIndex)
                {
                    return Directions.DOWN_RIGHT;
                }
                if (currentIndex + ((2 * currentRow) + 1) == targetSlot.slotIndex)
                {
                    return Directions.DOWN_LEFT;
                }
                if (currentIndex + 2 == targetSlot.slotIndex)
                {
                    return Directions.RIGHT;
                }
            }

            Debug.Log("Cannot move to given position.");
            return Directions.NONE;
        }
        else
        {
            Debug.Log("Peg is never assigned");
            return Directions.NONE;
        }
            
    }

    /// <summary>
    /// Returns a boolean indicating whether a peg has a neighbor in a given direction
    /// </summary>
    /// <param name="pegIndex"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public bool PegHasNeighbor(int pegIndex, Directions dir)
    {
        int ro = pegLayout[pegIndex].row;
        if (dir == Directions.UP_RIGHT)
        {
            if (pegLayout[(pegIndex - ro) + 1] != null)
            {
                jumpedPeg = pegLayout[(pegIndex - ro) + 1];
                return true;
            }
        }
        if (dir == Directions.UP_LEFT)
        {
            if (pegLayout[pegIndex - (ro)] != null)
            {
                jumpedPeg = pegLayout[pegIndex - (ro)];
                return true;
            }
        }
        if (dir == Directions.DOWN_LEFT)
        {
            if (pegLayout[pegIndex + (ro)] != null)
            {
                jumpedPeg = pegLayout[pegIndex + (ro)];
                return true;
            }
        }
        if (dir == Directions.DOWN_RIGHT)
        {
            if (pegLayout[pegIndex + (ro + 1)] != null)
            {
                jumpedPeg = pegLayout[pegIndex + (ro + 1)];
                return true;
            }
        }
        if (dir == Directions.LEFT)
        {
            if (pegLayout[pegIndex - 1] != null && pegLayout[pegIndex - 1].row == ro)
            {
                jumpedPeg = pegLayout[pegIndex - 1];
                return true;
            }
        }
        if (dir == Directions.RIGHT)
        {
            if (pegLayout[pegIndex + 1] != null && pegLayout[pegIndex + 1].row == ro)
            {
                jumpedPeg = pegLayout[pegIndex + 1];
                return true;
            }
        }
        return false;
    }

    private void MovePeg(int targetIndex, Directions dir)
    {
        bool canMoveInDir;

        if (currentPeg != null)
        {
            if (currentPeg.availableDirDict.TryGetValue(dir, out canMoveInDir))
            {
                if (canMoveInDir)
                {
                    if (!boardSlots[targetIndex].isFilled)
                    {
                        if (PegHasNeighbor(currentPeg.currentIndex, dir))
                        {
                            boardSlots[currentPeg.currentIndex].isFilled = false;
                            boardSlots[targetIndex].isFilled = true;
                            pegLayout[currentPeg.currentIndex] = null;

                            currentPeg.currentIndex = targetIndex;
                            currentPeg.row = boardSlots[targetIndex].row;
                            currentPeg.availableDirDict = boardSlots[targetIndex].moveDict;

                            boardSlots[jumpedPeg.currentIndex].isFilled = false;
                            pegLayout[jumpedPeg.currentIndex] = null;
                            pegLayout[targetIndex] = currentPeg;

                            currentPeg.SetMoving(boardSlots[currentPeg.currentIndex].indicator.transform.position, true);
                            Destroy(jumpedPeg.gameObject);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Move dictionary did not contain key for given direction.");
            }
        }
    }

    /// <summary>
    /// Checks whether a peg can move in a direction, 
    /// whether it has a neighbor in an adjacent slot, 
    /// and if the target position is currently occupied
    /// </summary>
    /// <returns></returns>
    private bool MovesExist()
    {
        for (int i = 0; i < pegLayout.Length; ++i)
        {
            if (pegLayout[i] == null)
            {
                continue;
            }
            else
            {
                bool uLeft  = pegLayout[i].availableDirDict[Directions.UP_LEFT]     == true;
                bool uRight = pegLayout[i].availableDirDict[Directions.UP_RIGHT]    == true;
                bool dLeft  = pegLayout[i].availableDirDict[Directions.DOWN_LEFT]   == true;
                bool dRight = pegLayout[i].availableDirDict[Directions.DOWN_RIGHT]  == true;
                bool left   = pegLayout[i].availableDirDict[Directions.LEFT]        == true;
                bool right  = pegLayout[i].availableDirDict[Directions.RIGHT]       == true;

                if (uLeft)
                {
                    if(PegHasNeighbor(pegLayout[i].currentIndex, Directions.UP_LEFT) 
                        && !boardSlots[TargetIndexFromDir(i, Directions.UP_LEFT)].isFilled)
                    {
                        return true;
                    }
                }
                if (uRight)
                {
                    if (PegHasNeighbor(pegLayout[i].currentIndex, Directions.UP_RIGHT) 
                        && !boardSlots[TargetIndexFromDir(i, Directions.UP_RIGHT)].isFilled)
                    {
                        return true;
                    }
                }
                if (dLeft)
                {
                    if (PegHasNeighbor(pegLayout[i].currentIndex, Directions.DOWN_LEFT) 
                        && !boardSlots[TargetIndexFromDir(i, Directions.DOWN_LEFT)].isFilled)
                    {
                        return true;
                    }
                }
                if (dRight)
                {
                    if(PegHasNeighbor(pegLayout[i].currentIndex, Directions.DOWN_RIGHT) 
                        && !boardSlots[TargetIndexFromDir(i, Directions.DOWN_RIGHT)].isFilled)
                    {
                        return true;
                    }
                }
                if (left)
                {
                    if(PegHasNeighbor(pegLayout[i].currentIndex, Directions.LEFT) 
                        && !boardSlots[i - 2].isFilled && (boardSlots[i - 2].row == pegLayout[i].row))
                    {
                        return true;
                    }
                }
                if (right)
                {
                    if(PegHasNeighbor(pegLayout[i].currentIndex, Directions.RIGHT) 
                        && !boardSlots[i + 2].isFilled && (boardSlots[i + 2].row == pegLayout[i].row))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

	void Update ()
    {
        if(GameStateManager._gameState != GameStateManager.GameState.RUNNING)
        {
            return;
        }

		if(Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (hit.transform != null && hit.transform.tag == "Peg")
            {
                if (currentPeg != null)
                {
                    currentPeg.Deselect();
                }
                currentPeg = hit.transform.GetComponent<Peg>();
                currentPeg.Select();
            }
            else
            {
                if (hit.transform != null && hit.transform.tag == "Slot")
                {
                    SlotIndicator tempIndicator = hit.transform.GetComponent<SlotIndicator>();
                    Slot s = boardSlots[tempIndicator.slotIndex];
                    if (!s.isFilled)
                    {
                        MovePeg(tempIndicator.slotIndex, DirectionFromTarget(tempIndicator));
                        if (!MovesExist())
                        {
                            int remainingPegs = 0;
                            for (int i = 0; i < pegLayout.Length; ++i)
                            {
                                if (pegLayout[i] != null)
                                {
                                    remainingPegs++;
                                }
                            }

                            if (remainingPegs == 1)
                            {
                                GameStateManager._gameState = GameStateManager.GameState.WIN;
                            }
                            else
                            {
                                GameStateManager._gameState = GameStateManager.GameState.LOSE;
                            }
                            Debug.Log("Remaining pegs " + remainingPegs);
                        }
                    }
                }
                if (currentPeg != null)
                {
                    currentPeg.Deselect();
                }
                currentPeg = null;
            }
        }
	}
}
