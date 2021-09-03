using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CarPartInfoHolder))]
public class CarPartInfoEditor : Editor
{
    private Texture _minusIcon;
    private Texture _plusIcon;
    private GUIContent _minusContent;
    private GUIContent _plusContent;

    FontStyle _originalLabelFont;

    public void Awake()
    {
        _minusIcon = Resources.Load<Texture>("MinusIcon");
        _plusIcon = Resources.Load<Texture>("PlusIcon");

        _minusContent = new GUIContent(" ", _minusIcon, "Remove Part");
        _plusContent = new GUIContent("Add Part", _plusIcon, "Add part to hide when focused");

        _originalLabelFont = EditorStyles.label.fontStyle; //save for quick recall later
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        CarPartInfoHolder cpih = (CarPartInfoHolder)target;

        //Check to make sure that this object has the Car Part tag
        if(cpih.gameObject.tag != "Car Part")
        {
            //Custom bolded gui style
            GUIStyle boldStyle = new GUIStyle();
            boldStyle.richText = true;

            EditorGUILayout.Space(15);
            EditorGUILayout.HelpBox("CRITICAL ERROR:\nThis GameObject needs to have the \"Car Part\" tag to function properly.", MessageType.Error);
            EditorGUILayout.Space(15);
        }

        EditorGUILayout.BeginHorizontal();
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Car Part Display Name:", GUILayout.Width(150));
        EditorStyles.label.fontStyle = _originalLabelFont;
        EditorGUILayout.Space(2);
        cpih.DisplayName = EditorGUILayout.TextField(cpih.DisplayName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(15);

        if(cpih.DisplayCameraHolder == null)
        {
            EditorGUILayout.HelpBox("CRITICAL ERROR:\nDisplay view not set.\nIn the scene view, position the camera focused on the target car part, " +
                "then hit \"Set Display View\".  This will set the viewing angle that will be used in the application.\n" +
                "I suggest working with the Game Screen visible to check placement.", MessageType.Error);
        }/*else
        {
            EditorGUILayout.HelpBox("Warning, \"Set Display View\" will override existing view.", MessageType.Warning);
        }*/

        EditorGUILayout.BeginHorizontal();
        if(cpih.DisplayCamera == null)
        {
            if (GUILayout.Button("Set Display View"))
            {
                //Create the display camera gameobject and add camera component to it;
                cpih.DisplayCameraHolder = new GameObject($"{cpih.DisplayName} Camera");
                cpih.DisplayCameraHolder.transform.SetParent(cpih.gameObject.transform.transform);
                cpih.DisplayCamera = cpih.DisplayCameraHolder.gameObject.AddComponent<Camera>();

                //Collect the scene camera and create a new camera based on it's position, rotation, etc.
                Camera sceneCam = UnityEditor.SceneView.lastActiveSceneView.camera;
                //Match the new camera to the scene camera
                if (sceneCam != null)
                {
                    cpih.DisplayCameraHolder.transform.eulerAngles = sceneCam.transform.eulerAngles;
                    cpih.DisplayCameraHolder.transform.rotation = sceneCam.transform.rotation;
                    cpih.DisplayCameraHolder.transform.position = sceneCam.transform.position; 
                }
            } 
        } else 
        {
            if(GUILayout.Button("Update Display View"))
            {
                Camera sceneCam = SceneView.lastActiveSceneView.camera;

                cpih.DisplayCameraHolder.transform.eulerAngles = sceneCam.transform.eulerAngles;
                cpih.DisplayCameraHolder.transform.rotation = sceneCam.transform.rotation;
                cpih.DisplayCameraHolder.transform.position = sceneCam.transform.position;
            }

            if (GUILayout.Button("Show Display View"))
            {
                Camera sceneCam = SceneView.lastActiveSceneView.camera;

                //Disable then re-enable this camera to make sure it has priority in the game window
                cpih.DisplayCamera.enabled = false;
                cpih.DisplayCamera.enabled = true;

                var sceneView = SceneView.lastActiveSceneView;
                if(sceneView != null)
                {
                    sceneView.AlignViewToObject(cpih.DisplayCameraHolder.transform);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Color.black);
        EditorGUILayout.Space(10);

        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Parts to hide while selected:");
        EditorStyles.label.fontStyle = _originalLabelFont;

        if(cpih.PartsToHideOnFocus == null || cpih.PartsToHideOnFocus.Count == 0)
        {
            
            EditorGUILayout.HelpBox("Parts that will be made transparent when this part is selected.", MessageType.Info);
        } else
        {
            for (int i = 0; i < cpih.PartsToHideOnFocus.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                cpih.PartsToHideOnFocus[i] = (CarPartInfoHolder)EditorGUILayout.ObjectField(cpih.PartsToHideOnFocus[i], typeof(CarPartInfoHolder), true, GUILayout.Width(300));
                if (GUILayout.Button(_minusContent, GUILayout.Width(32)))
                {
                    cpih.RemoveHiddenPart(i);
                    break; //Exit the loop for this update to avoid exceeding list length.
                }
                EditorGUILayout.EndHorizontal();
                if (cpih.PartsToHideOnFocus[i] != null && cpih.PartsToHideOnFocus[i].gameObject == cpih.gameObject)
                {
                    EditorGUILayout.HelpBox("ERROR: Including the target part as a part to be hidden will break the tool.  Please remove this part.", MessageType.Error);
                }
            }
        }

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button(_plusContent))
        {
            cpih.AddNewHiddenPart();
        }

        EditorGUILayout.EndHorizontal();

    }
}
   