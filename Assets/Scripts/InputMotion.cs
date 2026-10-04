using System.Collections.Generic;
using UnityEngine;

struct dirUnit
{
    public dirUnit(int newdir, int newwindow, bool newstrict)
    {
        diretion = newdir;
        window = newwindow;
        strict = newstrict;
    }
    public int diretion; // direction
    public int window; // frame window for next input
    public bool strict; // strict or loose direction matching
}

public class InputMotion : MonoBehaviour
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
    public static List<int> dirBuffer;
    bool faceLeft = false;

    public InputMotion Add(int direction, int window, bool strict)
    {
        inputList.Reverse();
        inputList.Add(new dirUnit(direction, window, strict));
        inputList.Reverse();
        return this;
    }

    //public bool checkValidInput(List<int> buffer, bool faceL = false)
    //{
    //    dirBuffer = buffer;
    //    faceLeft = faceL;
    //    return checkValidInput();
    //}

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
