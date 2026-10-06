using System.Collections.Generic;
using UnityEngine;

public struct dirUnit
{
    public dirUnit(int newdir, int newwindow, bool newstrict)
    {
        direction = newdir;
        window = newwindow;
        strict = newstrict;
    }
    public int direction; // direction
    public int window; // frame window for next input
    public bool strict; // strict or loose direction matching
}

public class InputMotion
{
    public InputMotion(string Name, List<dirUnit> newInputs)
    {
        name = Name;
        inputList = newInputs;
        inputList.Reverse(); // reverse the list so that the first input is at the end of the list

    }
    public InputMotion(string Name)
    {
        name = Name;
        inputList = new List<dirUnit>();
    }
    public string name;
    List<dirUnit> inputList;
    bool faceLeft = false; // might take away the ability to face left :skull:

    public InputMotion Add(int direction, int window, bool strict)
    {
        inputList.Reverse();
        inputList.Add(new dirUnit(direction, window, strict));
        inputList.Reverse();
        return this;
    }

    public bool CheckValidInput() 
    {
        return CheckValidInput(0, 0);
    }

    private bool CheckValidInput(int curInput, int bufferPos)
    {
        for(int i = bufferPos; i < bufferPos + inputList[curInput].window; i++)
        {
            if (CheckDir(InputReader.dirBuffer[i], inputList[curInput].direction, inputList[curInput].strict))
            {
                if (curInput + 1 == inputList.Count)
                {
                    Debug.Log(name);
                    return true;
                }
                else
                {
                    return CheckValidInput(curInput + 1, i + 1);
                }
            }
        }
        return false;
    }

    bool CheckDir(int curDir, int targetDir, bool strict)
    {
        //if (faceLeft)
        //{
        //    switch (curDir)
        //    {
        //        case 7:
        //        case 4:
        //        case 1:
        //        curDir += 2;
        //        break;
        //        case 9:
        //        case 6:
        //        case 3:
        //        curDir -= 2;
        //        break;
        //        default:
        //        break;
        //    }
        //}

        if (strict)
        {
            if(curDir == targetDir) return true;
            else return false;
        }
        else
        {
            if(targetDir == 6 && (curDir == 6 || curDir == 9 || curDir == 3)) return true; //target right
            else if(targetDir == 4 && (curDir == 4 || curDir == 1 || curDir == 7)) return true; //target left
            else if(targetDir == 8 && (curDir == 8 || curDir == 7 || curDir == 9)) return true; //target up
            else if(targetDir == 2 && (curDir == 2 || curDir == 1 || curDir == 3)) return true; //target down
            else if (targetDir == 3 && (curDir == 3 || curDir == 6 || curDir == 2)) return true; //target down-right
            else if (targetDir == 1 && (curDir == 1 || curDir == 4 || curDir == 2)) return true; //target down-left
            else if (targetDir == 9 && (curDir == 9 || curDir == 8 || curDir == 6)) return true; //target up-right
            else if (targetDir == 7 && (curDir == 7 || curDir == 8 || curDir == 4)) return true; //target up-left
        }
        return false;
    }
}
