using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragData
{
    int index;
    string windowContext;
    Inventory invFrom;

    public DragData(int index, string windowContext, Inventory invFrom)
    {
        this.index = index;
        this.windowContext = windowContext;
        this.invFrom = invFrom;
    }

    public int Index {
        get
        {
            return this.index;
        }
    }

    public string WindowContext {
        get
        {
            return this.windowContext;
        }
    }

    public Inventory InvFrom 
    {
        get
        {
            return this.invFrom;
        }
    }
}
