using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    //Delegate to handle passing information to the interaction
    //handler when this button is pressed.
    public delegate void OnCarPartButtonClickedDelegate(GameObject g);
    public static OnCarPartButtonClickedDelegate carPartButtonClickDelegate;

    //The gameobject (Car Part) this button is referencing
    private GameObject _carPart;

    // Start is called before the first frame update
    /// <summary>
    /// Initialize the new button
    /// </summary>
    /// <param name="name">Name of the part attached tot his button</param>
    /// <param name="carPart">The gameobject this button is referencing</param>
    public void init(string name, GameObject carPart)
    {
        this.name = name;
        _carPart = carPart;

        this.GetComponentInChildren<Text>().text = name;
    }

    public void OnCarPartButtonClicked()
    {
        carPartButtonClickDelegate(_carPart);
    }

}
