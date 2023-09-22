using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonSelector : MonoBehaviour
{
    public int index;
    public ManagerScr manager;

    public void SelectThisCategory()
    {
        manager.SelectCategory(index);
    }
}
