using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is used to hold information about the given car part that
 * will be accessed by various scripts.  It will be automatically generated
 * at runtime and attached to any object with the Car Part tag.
 */

[System.Serializable]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CarPartInfoHolder : MonoBehaviour
{
    [SerializeField]
    private string _displayName;
    public string DisplayName
    {
        get { return _displayName; }
        set { _displayName = value; }
    }
    [SerializeField]
    private GameObject _displayCameraHolder;
    public GameObject DisplayCameraHolder
    {
        get { return _displayCameraHolder;}
        set { _displayCameraHolder = value; }
    } 

    //Note: While the holder object would be enough to store the position and rotation data needed for
    //positioning the camera during runtime, I am opting to also use a camera to make it easy to debug 
    //the view in the game window without moving the game camera from its starting position.
    [SerializeField]
    private Camera _displayCamera;
    public Camera DisplayCamera
    {
        get { return _displayCamera; }
        set { _displayCamera = value; }
    }

    //Parts to hide when this object is in focus
    [SerializeField]
    private List<CarPartInfoHolder> _partsToHideOnFocus;
    public List<CarPartInfoHolder> PartsToHideOnFocus
    {
        get { return _partsToHideOnFocus; }
        set { _partsToHideOnFocus = value; }
    }

    //Store the default material(s) for the Car Part for easy re-application.
    [SerializeField]
    private MeshRenderer _mr;
    [SerializeField]
    private Material[] _materials;
    /// <summary>
    /// Material applied when there is a different carpart selected.  Makes the rest of the car transparent.
    /// </summary>
    [SerializeField]
    private Material _hiddenMaterial;
    /// <summary>
    /// Material used when the car part is hovered over.
    /// </summary>
    [SerializeField]
    private Material _hoverMaterial;
    /// <summary>
    /// Material used when the car part if hovered over but also hidden
    /// </summary>
    [SerializeField]
    private Material _hoverHiddenMaterial;
    /// <summary>
    /// Material used when the car part is currently selected
    /// </summary>
    [SerializeField]
    private Material _selectedMaterial;


    /// <summary>
    /// Is this part currently hidden due to another part?
    /// </summary>
    private bool _isHidden;
    public bool IsHidden
    {
        get { return _isHidden; }
        set { _isHidden = value; }
    }

    /// <summary>
    /// Initialize the car part info holder.  This will collect pertinent information at runtime (materials).
    /// </summary>
    public void init()
    {
        _mr = this.gameObject.GetComponent<MeshRenderer>();
        _materials = _mr.materials;

        //Load the hidden object material
        _hiddenMaterial = Resources.Load<Material>("HiddenMaterial");
        _hoverHiddenMaterial = Resources.Load<Material>("HighlightHiddenHoverMaterial");
        _hoverMaterial = Resources.Load<Material>("HighlightHoverMaterial");
        _selectedMaterial = Resources.Load<Material>("HighlightSelectionMaterial");
    }

    /// <summary>
    /// Called when a Car Part object doesn't have this script attached at runtime.
    /// </summary>
    /// <param name="name">Display name for this car part</param>
    public void faultyInit(string name)
    {
        _displayName = name;
    }

    public void SetAsSelected()
    {
        //Copy material textres and other variables that control appearance
        //Always use _material[0] instead of _mr.material in the event that
        //the partwas hidden prior to selection.  Bing hidden removes all
        //textures and other information for clarity.
        CopyProperties(_materials[0], _selectedMaterial, false);

        _mr.material = _selectedMaterial;
        if (_partsToHideOnFocus != null)
        {
            foreach(CarPartInfoHolder part in _partsToHideOnFocus)
            {
                if(part != null)
                {
                    part.ApplyHiddenMaterial();
                }
            }
        }
    }

    public void SetAsHovered()
    {
        if (_isHidden)
        {
            //Hidden objects should continue to lack detail when hoevered over.  If we decide
            //we would rather have them show detail, uncomment this line.
            //CopyProperties(_materials[0], _hoverHiddenMaterial);
            _mr.material = _hoverHiddenMaterial;
        } else
        {
            CopyProperties(_materials[0], _hoverMaterial, false);
            _mr.material = _hoverMaterial;
        }
    }

    public void DeselectPart()
    {
        _mr.materials = _materials;
        if (_partsToHideOnFocus != null)
        {
            foreach (CarPartInfoHolder part in _partsToHideOnFocus)
            {
                part.RemoveHiddenMaterial();
            }
        }
    }
    /// <summary>
    /// Reset the Car Part to it's original material or the hidden material, as appropriate
    /// </summary>
    public void ResetMaterials()
    {
        if (_isHidden)
        {
            _mr.material = _hiddenMaterial;
        } else
        {
            _mr.materials = _materials;
        }
    }

    /// <summary>
    /// Copy maps and floats from m1 to m2, used to match the hover and select materials to the originals (for detailing)
    /// </summary>
    /// <param name="m1">The material being copied from</param>
    /// <param name="m2">The material being copied to</param>
    private void CopyProperties(Material m1, Material m2, bool isTransparent)
    {
        //Different shaders may have different properties.  This is build on the default Unity Shader.
        if (m1.shader == Shader.Find("Standard (Specular setup)"))
        {
            if (!isTransparent)
            {
                SetMaterialOpaque(m2);
            }
            try
            {
                m2.SetTexture("_MainTex", m1.GetTexture("_MainTex"));
                m2.SetFloat("_Glossiness", m1.GetFloat("_Glossiness"));
                m2.SetFloat("_GlossMapScale", m1.GetFloat("_GlossMapScale"));
                m2.SetFloat("_SmoothnessTextureChannel", m1.GetFloat("_SmoothnessTextureChannel"));
                m2.SetColor("_SpecColor", m1.GetColor("_SpecColor"));
                m2.SetTexture("_SpecGlossMap", m1.GetTexture("_SpecGlossMap"));
                m2.SetFloat("_SpecularHighlights", m1.GetFloat("_SpecularHighlights"));
                m2.SetFloat("_GlossyReflections", m1.GetFloat("_GlossyReflections"));
                m2.SetFloat("_BumpScale", m1.GetFloat("_BumpScale"));
                m2.SetTexture("_BumpMap", m1.GetTexture("_BumpMap"));
                m2.SetFloat("_Parallax", m1.GetFloat("_Parallax"));
                m2.SetTexture("_ParallaxMap", m1.GetTexture("_ParallaxMap"));
                m2.SetFloat("_OcclusionStrength", m1.GetFloat("_OcclusionStrength"));
                m2.SetTexture("_OcclusionMap", m1.GetTexture("_OcclusionMap"));
                m2.SetColor("_EmissionColor", m1.GetColor("_EmissionColor"));
                m2.SetTexture("_EmissionMap", m1.GetTexture("_EmissionMap"));
                m2.SetTexture("_DetailMask", m1.GetTexture("_DetailMask"));
                m2.SetTexture("_DetailAlbedoMap", m1.GetTexture("_DetailAlbedoMap"));
                m2.SetFloat("_DetailNormalMapScale", m1.GetFloat("_DetailNormalMapScale"));
                m2.SetTexture("_DetailNormalMap", m1.GetTexture("_DetailNormalMap"));

            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }

            return;
        }
        //Standard shader (non-specular) property manager.
        if (m1.shader == Shader.Find("Standard"))
        {
            Debug.Log("Standard");
            try
            {
                m2.SetTexture("_MainTex", m1.GetTexture("_MainTex"));
                m2.SetFloat("_Glossiness", m1.GetFloat("_Glossiness"));
                m2.SetFloat("_GlossMapScale", m1.GetFloat("_GlossMapScale"));
                m2.SetFloat("_SmoothnessTextureChannel", m1.GetFloat("_SmoothnessTextureChannel"));
                m2.SetFloat("_Metallic", m1.GetFloat("_Metallic"));
                m2.SetTexture("_MetallicGlossMap", m1.GetTexture("_MetallicGlossMap"));
                m2.SetFloat("_SpecularHighlights", m1.GetFloat("_SpecularHighlights"));
                m2.SetFloat("_GlossyReflections", m1.GetFloat("_GlossyReflections"));
                m2.SetFloat("_BumpScale", m1.GetFloat("_BumpScale"));
                m2.SetTexture("_BumpMap", m1.GetTexture("_BumpMap"));
                m2.SetFloat("_Parallax", m1.GetFloat("_Parallax"));
                m2.SetTexture("_ParallaxMap", m1.GetTexture("_ParallaxMap"));
                m2.SetFloat("_OcclusionStrength", m1.GetFloat("_OcclusionStrength"));
                m2.SetTexture("_OcclusionMap", m1.GetTexture("_OcclusionMap"));
                m2.SetColor("_EmissionColor", m1.GetColor("_EmissionColor"));
                m2.SetTexture("_EmissionMap", m1.GetTexture("_EmissionMap"));
                m2.SetTexture("_DetailMask", m1.GetTexture("_DetailMask"));
                m2.SetTexture("_DetailAlbedoMap", m1.GetTexture("_DetailAlbedoMap"));
                m2.SetFloat("_DetailNormalMapScale", m1.GetFloat("_DetailNormalMapScale"));
                m2.SetTexture("_DetailNormalMap", m1.GetTexture("_DetailNormalMap"));

            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return;
        }

        //Special case for the headlights and brake lights.  These are a mobile shader with
        //only the _mainTex property.  Clear all other texture properties.
        //In order to approximate the transparent look it is necessary that the sprite used in
        //The example car have "alpha from greyscale" selected.  For other models this may need
        //to be switched to "Alpha from Transparency" depending on how the texture file us set up.
        if (m1.shader == Shader.Find("Mobile/Particles/Additive"))
        {
            try
            {
                //This must be set to transparent.
                SetMaterialTransparent(m2);

                m2.SetTexture("_MainTex", m1.GetTexture("_MainTex"));

                m2.SetTexture("_SpecGlossMap", null);
                //For transparency to work, this must be set to "Specular Alpha", which is 0 in the shader.
                m2.SetFloat("Smoothness texture channel", 0);
                m2.SetTexture("_BumpMap", null);
                m2.SetTexture("_ParallaxMap", null);
                m2.SetTexture("_OcclusionMap", null);
                m2.SetTexture("_EmissionMap", null);
                m2.SetTexture("_DetailMask", null);
                m2.SetTexture("_DetailAlbedoMap", null);
                m2.SetTexture("_DetailNormalMap", null);
            }catch(System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return;
        }

        //Note: Any additional types of shaders used would require a similar copy method, but once set
        //up should work for all like-sharders moving forward.
    }

    private void SetMaterialOpaque(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }

    private void SetMaterialTransparent(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    public void AddNewHiddenPart()
    {
        if(_partsToHideOnFocus == null)
        {
            _partsToHideOnFocus = new List<CarPartInfoHolder>();
        }
        _partsToHideOnFocus.Add(null);
    }

    public void RemoveHiddenPart(int index)
    {
        if(_partsToHideOnFocus != null)
        {
            _partsToHideOnFocus.RemoveAt(index);
        }
    }

    public void ApplyHiddenMaterial()
    {
        _mr.material = _hiddenMaterial;
        _isHidden = true;
    }

    public void RemoveHiddenMaterial()
    {
        _mr.materials = _materials;
        _isHidden = false;
    }
}
