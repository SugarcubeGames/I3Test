using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    /// <summary>
    /// The last object that was hit by the raycast.
    /// </summary>
    private GameObject _lastHit;
    /// <summary>
    /// The object that is currently selected
    /// </summary>
    private GameObject _selectedObject;

    //Reference to the scene's main camera.  Used for camera control.
    public GameObject _mainCamera;

    [Header("UI Element References")]
    public GameObject _currentSelectionPanel;
    public Text _currentSelectionNameText;
    private bool _currentSelectionPanelIsActive;

    [Space(2)]
    public GameObject _currentHoverPanel;
    public Text _currentHoverNameText;
    //References a secondary label panel which displays the name of the object being hovered over
    //in the top right corner.  Uncomment this declaration and all other references to reenable.
    //private bool _currentHoverPanelIsActive;

    [Space(2)]
    [Header("Transition Variables")]
    [Range(0,2)]
    public float _lerpSpeed = 1f;
    private Transform _lerpTargetTransform;
    private bool _isLerping;

    [Space(2)]
    [Header("Camera Rotation Variables")]
    [Range(1, 10)]
    public float _speedRotate = 5f;
    [Range(1, 5)]
    public float _speedZoom = 1f;
    [Range(.1f, 2)]
    public float _speedPan = .5f;

    private Vector3 _mouseOrigin;
    private Vector3 _rotateHitPos; //Where the raycast for rotation hits.  Point to be rotated around.
    private bool _isPanning;
    private bool _isRotating;
    //Did we hit something when casting raytrace for rotation?
    //Determines the rotation method.
    private bool _rotationHitObject;

    // Start is called before the first frame update
    void Start()
    {
        _lastHit = null;
        //_currentHoverPanelIsActive = false;
        _currentSelectionPanelIsActive = false;

        _currentHoverPanel.SetActive(false);
        _currentSelectionPanel.SetActive(false);

        _isLerping = false;

        _isPanning = false;
        _isRotating = false;
        _rotationHitObject = false;

        ///Subscribe to the OnCarPartButtonClicked method
        ButtonHandler.carPartButtonClickDelegate += OnCarPartButtonClicked;
    }

    // Update is called once per frame
    void Update()
    {

        //Manage Camera
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        //_mainCamera.transform.Translate(0, scroll * -_speedZoom, scroll * -_speedZoom, Space.World);
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            var r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit h;
            Vector3 targetPos;

            if(Physics.Raycast(r, out h))
            {
                targetPos = h.point;
            } else
            {
                //If we hit nothing, just move forward
                targetPos = Camera.main.transform.forward * _speedZoom;
            }

            float dis = Vector3.Distance(targetPos, Camera.main.transform.position);
            Vector3 dir = Vector3.Normalize(targetPos - Camera.main.transform.position)
                * (dis * Input.GetAxis("Mouse ScrollWheel"));

            Camera.main.transform.position += dir;

            //If mouse movement is enacted, cancel any lerping currenly going on.
            _isLerping = false;
        }

        //Rotate camera with right click
        if (Input.GetMouseButtonDown(1))
        {
            _isRotating = true;
            _mouseOrigin = Input.mousePosition;

            var r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit h;

            if(Physics.Raycast(r, out h))
            {
                //We hti a gameobject, rotate aroung it
                _rotationHitObject = true;
                _rotateHitPos = h.point;
            } else
            {
                _rotationHitObject = false;
            }

            //If mouse movement is enacted, cancel any lerping currenly going on.
            _isLerping = false;
        }

        //Pan camera with center click on scrollwheel
        if (Input.GetMouseButtonDown(2))
        {
            _isPanning = true;
            _mouseOrigin = Input.mousePosition;

            //If mouse movement is enacted, cancel any lerping currenly going on.
            _isLerping = false;
        }

        if (Input.GetMouseButtonUp(1)) _isRotating = false;
        if (Input.GetMouseButtonUp(2)) _isPanning = false;

        if (_isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);

            Transform ct = Camera.main.transform;
            //If we hit something, rotate around it, otherwise rotate around the camera position
            if (_rotationHitObject)
            {
                ct.RotateAround(_rotateHitPos, ct.right, -pos.y * _speedRotate);
                ct.RotateAround(_rotateHitPos, Vector3.up, pos.x * _speedRotate);
            } else
            {
                ct.RotateAround(ct.position, ct.right, -pos.y * _speedRotate);
                ct.RotateAround(ct.position, Vector3.up, pos.x * _speedRotate);
            }

            //Update the mouse position to prevent constant rotation
            //_mouseOrigin = Input.mousePosition;
        }

        if (_isPanning)
        {
           
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);

            Camera.main.transform.Translate(pos * -_speedPan);
            //Update the mouse position to prevent constant pan
            _mouseOrigin = Input.mousePosition;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Car Part")
        {
            //Make sure that we're not over the ui
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //If our mouse has hovered over a new Car Part, update the hovered object.
            if (hit.collider.gameObject != _lastHit  && hit.collider.gameObject != _selectedObject)
            {
                if(_lastHit != null)
                {
                    _lastHit.GetComponent<CarPartInfoHolder>().ResetMaterials();
                }

                _lastHit = hit.collider.gameObject;
                CarPartInfoHolder cpih = _lastHit.GetComponent<CarPartInfoHolder>();

                //Update the interfact to show the name of the object that's being hovered over
                _currentHoverNameText.text = cpih.DisplayName;

                //Apply the hover material to the object being hovered over.
                cpih.SetAsHovered();

                /*
                //If the hover panel isn't active, show it
                if (!_currentHoverPanelIsActive)
                {
                    _currentHoverPanel.SetActive(true);
                    _currentHoverPanelIsActive = true;
                }
                */
            } else if(hit.collider.gameObject != _lastHit && hit.collider.gameObject == _selectedObject)
            {
                if(_lastHit == null)
                {
                    return;
                }
                //If hovering over the selected object, clear the last hit object and reset it's material
                _lastHit.GetComponent<CarPartInfoHolder>().ResetMaterials();
                _lastHit = null;
                //_currentHoverPanel.SetActive(false);
                //_currentHoverPanelIsActive = false;
            }

            //If we are hovering over a selectable object and the mouse button is pressed make that object the new selected object
            //and move the camera to focus on it, based on the focus set in the scene editor.
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject != _selectedObject)
            {
                //If there's already a selected object, reset it's material.
                if(_selectedObject != null)
                {
                    _selectedObject.GetComponent<CarPartInfoHolder>().DeselectPart();
                }

                _selectedObject = hit.collider.gameObject;
                CarPartInfoHolder cpih = _selectedObject.GetComponent<CarPartInfoHolder>();

                //Set the material to the selected highlight material
                cpih.SetAsSelected();

                //To prevent issues of overwriting materials when hovering over a new car part, clear the hover variables
                _lastHit = null;
                
                _currentSelectionNameText.text = cpih.DisplayName;

                if (!_currentSelectionPanelIsActive)
                {
                    _currentSelectionPanel.SetActive(true);
                    _currentSelectionPanelIsActive = true;
                }

                _lerpTargetTransform = cpih.DisplayCameraHolder.transform;

                //Mark lerping as true so we'll start moving
                _isLerping = true;

            }
        } 
        else
        {
            //If we're not hitting anything, hide the hover pane and clear lasthit
            //If we were just hovering over the now-selected object, don't do anything
            if(_lastHit == _selectedObject)
            {
                return;
            }

            //Reset the previously-hovered object's material
            if(_lastHit != null)
            {
                _lastHit.GetComponent<CarPartInfoHolder>().ResetMaterials();
            }

            _lastHit = null;
            //_currentHoverPanel.SetActive(false);
            //_currentHoverPanelIsActive = false;

            //If we're not hitting anything and the user clicks, deselect the selected object
            //Also, make sure we're not clicking on the GUI.
            if (Input.GetMouseButtonDown(0)  && !EventSystem.current.IsPointerOverGameObject())
            {
                _selectedObject.GetComponent<CarPartInfoHolder>().DeselectPart();
                _selectedObject = null;
                _currentSelectionPanel.SetActive(false);
                _currentSelectionPanelIsActive = false;
            }
        }

    }

    void LateUpdate()
    {
        //Lerp Position
        if(_lerpTargetTransform != null  && _isLerping)
        {
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _lerpTargetTransform.position, Time.deltaTime * _lerpSpeed);
            //_mainCamera.transform.eulerAngles = Vector3.Lerp(_mainCamera.transform.eulerAngles, _lerpTargetTransform.eulerAngles, Time.deltaTime * _lerpSpeed);
            _mainCamera.transform.rotation = Quaternion.Lerp(_mainCamera.transform.rotation, _lerpTargetTransform.rotation, Time.deltaTime * _lerpSpeed);

            //Check for if we're within the lerp threshold (close enough to stop)
            if (Vector3.Distance(_lerpTargetTransform.position, _mainCamera.transform.position) < .01)
            {
                _isLerping = false;
            }
        }
    }

    public void OnCarPartButtonClicked(GameObject g)
    {
       
        //If the passed-in object is already selected, there is nothing we need to do.
        if(g == _selectedObject)
        {
            return;
        }

        //If not, run through the same process as when we click on an object to select it.
        //Deselect the currently-selected object, if one exists.
        //Reset the selected object's material
        if (_selectedObject != null)
        {
            _selectedObject.GetComponent<CarPartInfoHolder>().DeselectPart();
        }

        _selectedObject = g;
        CarPartInfoHolder cpih = g.GetComponent<CarPartInfoHolder>();

        //Set the material to the selected highlight material
        //cpih.SetMateral(_highlightSelectMaterial);
        cpih.SetAsSelected();

        //To prevent issues of overwriting materials when hovering over a new car part, clear the hover variables
        _lastHit = null;

        _currentSelectionNameText.text = cpih.DisplayName;

        if (!_currentSelectionPanelIsActive)
        {
            _currentSelectionPanel.SetActive(true);
            _currentSelectionPanelIsActive = true;
        }

        //Set the target position information to the slected part's camera's transform
        _lerpTargetTransform = cpih.DisplayCameraHolder.transform;

        //Mark lerping as true so we'll start moving
        _isLerping = true;
    }
}
