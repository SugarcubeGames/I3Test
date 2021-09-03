using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class UIGenerator : MonoBehaviour
{
    /// <summary>
    /// Prefab for Car Part button (for auto-generated list)
    /// </summary>
    [Header("Prefab References")]
    public GameObject _partButtonPrefab;

    /// <summary>
    /// Scrollview Content which holds all the auto-generated buttons
    /// </summary>
    [Space(5)]
    [Header("UI Object References")]
    public GameObject _buttonHolderScrollviewContent;

    /// <summary>
    /// Array of all object marked as "Car Part"  Used to generate tags
    /// </summary>
    private GameObject[] _carParts;

    // Start is called before the first frame update
    void Awake()
    {
        //Collect all objectsw that have been given the tag "Car Part"
        _carParts = GameObject.FindGameObjectsWithTag("Car Part");

        //Check to make sure that each object has teh required CarPartsInfo script

        List<string> _faultyObjects = new List<string>(); 
        foreach(GameObject o in _carParts)
        {
            CarPartInfoHolder cpio = o.GetComponent<CarPartInfoHolder>();
            if (cpio == null)
            {
                _faultyObjects.Add(o.name);

                //If it doesn't exist, create it and add it.  A warning will be
                //Thrown one all have been checked to let the developer
                //know they need to add that script.
                cpio = o.AddComponent(typeof(CarPartInfoHolder)) as CarPartInfoHolder;
                cpio.faultyInit(o.name);
            }

            //Run the init method to collect material information and other info to be stored at runtime.
            cpio.init();
        }
        //If any car parts are missing the CarPartInfo script, throw a warning to let the dev know
        if (_faultyObjects.Count > 0)
        {
            string error = "The following parts are missing the required CarPartsInfoHandler script.\n";
            foreach (string s in _faultyObjects)
            {
                error += "\t" + s + "\n";
            }
            error += "Placeholders have been added for this session. Please ensure that these are added prior to deployment.\n";

            Debug.LogError(error);
        }

        //Sort the gameobjects in the Array by their name.
        IComparer icomparer = new SortByNameComparer();
        Array.Sort(_carParts, icomparer);
        
        foreach(GameObject o in _carParts)
        {
            GameObject go = GameObject.Instantiate(_partButtonPrefab);

            //CarPartInfoHolder cpio = o.AddComponent<CarPartInfoHolder>();
            //CarPartInfoHolder cpio = go.AddComponent(typeof(CarPartInfoHolder)) as CarPartInfoHolder;

            CarPartInfoHolder cpio = o.GetComponent<CarPartInfoHolder>();

            go.GetComponent<ButtonHandler>().init(cpio.DisplayName, o);
            go.transform.SetParent(_buttonHolderScrollviewContent.transform);
            //go.transform.SetParent(this.transform);
        }
    }
}
