using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Comparer to manage sorting the car part gameobjects by name;
public class SortByNameComparer : IComparer
{
    /*
    int IComparer.Compare(object x, object y)
    {
        return ((new CaseInsensitiveComparer()).Compare(((GameObject)x).name, 
            ((GameObject)y).name));
    }
    */
    //Comapre the objects based on their display name
    int IComparer.Compare(object x, object y)
    {
        return ((new CaseInsensitiveComparer()).
            Compare(((GameObject)x).GetComponent<CarPartInfoHolder>().DisplayName,
                    ((GameObject)y).GetComponent<CarPartInfoHolder>().DisplayName));
    }
}
