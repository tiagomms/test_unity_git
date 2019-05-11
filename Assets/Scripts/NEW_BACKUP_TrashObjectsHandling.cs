using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEW_BACKUP_TrashObjectsHandling : MonoBehaviour {

    // PERFORMANCE IMPROVEMENTS: change matIndexes to arrays of each material
    private const string TRASH_MATERIALS_PATH = "Materials/Shared/TrashMaterials/";
    public const float notConsciousScaleIncrease = 1.2f;
    private const float notConsciousMaxScale = 2.1f;

    public enum TrashGOMode
    {
        NORMAL, SELECTED, FADING, REAPPEARING, ALMOST_GONE, INACTIVE
    }
    public class TrashGODetails
    {
        private int[] matIndexes;
        private GameObject gObject;
        private Renderer goRender;
        private TrashGOMode goMode = TrashGOMode.NORMAL;
        private TrashGOMode previousGOMode = TrashGOMode.NORMAL;
        private Vector3 maxGOScale = new Vector3();
        private bool isHit = false;
        public TrashGOMode GOMode
        {
            get
            {
                return goMode;
            }

            set
            {
                goMode = value;
            }
        }

        public int[] MatIndexes
        {
            get
            {
                return matIndexes;
            }

            set
            {
                matIndexes = value;
            }
        }
        
        public Renderer GORender
        {
            get
            {
                return goRender;
            }

            set
            {
                goRender = value;
            }
        }

        public bool IsHit
        {
            get
            {
                return isHit;
            }

            set
            {
                isHit = value;
            }
        }

        public GameObject GObject
        {
            get
            {
                return gObject;
            }

            set
            {
                gObject = value;
            }
        }

        public TrashGOMode PreviousGOMode
        {
            get
            {
                return previousGOMode;
            }

            set
            {
                previousGOMode = value;
            }
        }

        public Vector3 MaxGOScale
        {
            get
            {
                return maxGOScale;
            }

            set
            {
                maxGOScale = value;
            }
        }
    }


    public Dictionary<string, TrashGODetails> trashGODict = new Dictionary<string, TrashGODetails>();
    public Material[] normalMaterials;
    public Material[] selectedMaterials;
    public Material[] fadingMaterials;
    private Material[] almostGoneMaterials;

    // utility if all objects are normal and disabled, no need to run the update function
    private int nbrNormalOrInactiveObjects = 0;    
    private float beforeFadingRimPower = 3.0f;
    public float becomingConsciousFadingAlphaDecrease = 0.75f;
    private float minObjectFadingAlpha = 0f;
    
    
    // private LTDescr fadingAnimationLT;
    // private int fadingAnimationId = 0;
    // private LTDescr selectedAnimationLT;
    // private int selectedAnimationId = 0;


    private void Awake()
    {
        SetupTrashObjectsAndMaterials();
    }

    private void SetupTrashObjectsAndMaterials()
    {
        string animatedMaterialsPath = TRASH_MATERIALS_PATH + "CONSCIOUSNESS_LEVEL/" + Global.GetConsciousnessLevelString() + "/";
        // setup materials - load them from their respective folders
        normalMaterials = Resources.LoadAll<Material>(TRASH_MATERIALS_PATH + "0_NORMAL/");
        selectedMaterials = Resources.LoadAll<Material>(animatedMaterialsPath + "1_SELECTED/");
        fadingMaterials = Resources.LoadAll<Material>(animatedMaterialsPath + "2_FADING/");

        // if consciousness level is becoming, objects do not fade completely
        if (Global.ConsciousLevel == Global.ConsciousnessLevel.BECOMING) {
            minObjectFadingAlpha = becomingConsciousFadingAlphaDecrease;
            almostGoneMaterials = Resources.LoadAll<Material>(animatedMaterialsPath + "3_ALMOST_GONE/");
        }

        GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("objectsToClean");
        foreach (GameObject obj in trashObjects)
        {
            TrashGODetails trashGODetails = new TrashGODetails();
            Renderer rend = obj.GetComponent<Renderer>();
            int[] matIndexes = new int[rend.sharedMaterials.Length];
            int curMatIndex = 0;
            int i = 0;
            
            while (curMatIndex < matIndexes.Length) {
                if (rend.sharedMaterials[curMatIndex].name.Contains(normalMaterials[i].name)) {
                    matIndexes[curMatIndex] = i;                    
                    curMatIndex++;

                    i = -1;
                }
                i++;
            }
            
            trashGODetails.GORender = rend;
            trashGODetails.MatIndexes = matIndexes;
            trashGODetails.GObject = obj;
            trashGODetails.MaxGOScale = obj.transform.localScale * notConsciousMaxScale;
            
            trashGODict.Add(obj.name, trashGODetails);
        }
        nbrNormalOrInactiveObjects = trashGODict.Count;
        
    }

    private void Start()
    {
        SelectedMaterialsAnimation_Part1();
    }

    /*
     * SelectedMaterialsAnimation is a continuous animation
     */
    private void SelectedMaterialsAnimation_Part1()
    {
        LeanTween.value(gameObject, 3f, 0.1f, 2f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)SelectedMaterialsAnimationUpdate)
            .setOnComplete(SelectedMaterialsAnimation_Part2);
    }

    private void SelectedMaterialsAnimation_Part2()
    {
        LeanTween.value(gameObject, 0.1f, 3f, 2f)
            .setDelay(0.2f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)SelectedMaterialsAnimationUpdate)
            .setOnComplete(SelectedMaterialsAnimation_Part1);
    }

    private void SelectedMaterialsAnimationUpdate(float value)
    {
        foreach (Material sMat in selectedMaterials)
        {
            sMat.SetFloat("_RimPower", value);
        }
    }


    private void CreateFadingMaterialsAnimation()
    {   
        // before fading animation, get the rim power of all objects
        beforeFadingRimPower = selectedMaterials[0].GetFloat("_RimPower");

        LeanTween.value(gameObject, 1f, minObjectFadingAlpha, 3f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)FadingMaterialsAnimationUpdate)
            .setOnComplete(FadingMaterialsAnimationComplete);
        
    }
    private void FadingMaterialsAnimationComplete()
    {
        if (Global.ConsciousLevel == Global.ConsciousnessLevel.FULLY) {
            // trigger fading objects to inactive
            TriggerInactive();
            // reset fading materials - set alpha to 1
            FadingMaterialsAnimationUpdate(1f);
        } else if (Global.ConsciousLevel == Global.ConsciousnessLevel.NOT) {
            TriggerReappearing();
        } else if (Global.ConsciousLevel == Global.ConsciousnessLevel.BECOMING) {
            TriggerAlmostGone();
            AlmostGoneMaterialsAnimationUpdate(minObjectFadingAlpha);
            // reset fading materials
            FadingMaterialsAnimationUpdate(1f);
            // decrease min Object Fading Alpha
            minObjectFadingAlpha = Mathf.Max(minObjectFadingAlpha * becomingConsciousFadingAlphaDecrease, 0.2f);
        }
    }

    private void FadingMaterialsAnimationUpdate(float value)
    {
        foreach(Material fMat in fadingMaterials) {
            Color tintColor = fMat.GetColor("_ColorTint");
            Color rimColor = fMat.GetColor("_RimColor");
            Color outlineColor = fMat.GetColor("_OutlineColor");

            tintColor.a = value;
            outlineColor.a = value;
            rimColor.a = value;

            fMat.SetColor("_ColorTint", tintColor);
            fMat.SetColor("_RimColor", rimColor);
            fMat.SetColor("_OutlineColor", outlineColor);

            // rim power based on before fading value;
            fMat.SetFloat("_RimPower", beforeFadingRimPower);
        }
    }
    private void AlmostGoneMaterialsAnimationUpdate(float value)
    {
        foreach(Material fMat in almostGoneMaterials) {
            Color tintColor = fMat.GetColor("_ColorTint");
            Color rimColor = fMat.GetColor("_RimColor");
            Color outlineColor = fMat.GetColor("_OutlineColor");

            tintColor.a = value;
            outlineColor.a = value;
            rimColor.a = value;

            fMat.SetColor("_ColorTint", tintColor);
            fMat.SetColor("_RimColor", rimColor);
            fMat.SetColor("_OutlineColor", outlineColor);

            // rim power based on before fading value;
            fMat.SetFloat("_RimPower", beforeFadingRimPower);
        }
    }

    // only for levels with no conscience, the object appears to reappear
    private void CreateReappearingMaterialsAnimation()
    {
        LeanTween.value(gameObject, 0f, 1f, 1.5f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)FadingMaterialsAnimationUpdate)
            .setOnComplete(ReappearingMaterialsAnimationComplete);
        
    }

    private void ReappearingMaterialsAnimationComplete()
    {
        TriggerBackToNormal(TrashGOMode.REAPPEARING);
    }

    private void changeSharedMaterials(TrashGODetails objDetails, Material[] materialsArray) {
        Material[] newSharedMaterials = new Material[objDetails.MatIndexes.Length];
        for (int i = 0; i < objDetails.MatIndexes.Length; i++) {
            newSharedMaterials[i] = materialsArray[objDetails.MatIndexes[i]];
        }
        objDetails.GORender.sharedMaterials = newSharedMaterials;
    }

    private void ChangeTrashObjectToNormal(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.NORMAL;
        changeSharedMaterials(objDetails, normalMaterials);
        
        // increase Normal Objects;
        nbrNormalOrInactiveObjects++;
    }

    private void ChangeTrashObjectToSelected(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.SELECTED;
        changeSharedMaterials(objDetails, selectedMaterials);

        // decrease normal objects;
        nbrNormalOrInactiveObjects--;        
    }
    private void ChangeTrashObjectToFading(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.FADING;
        changeSharedMaterials(objDetails, fadingMaterials);
    }

    private void ChangeTrashObjectToInactive(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GObject.SetActive(false);

        changeSharedMaterials(objDetails, normalMaterials);
        
        // increase Normal Objects;
        nbrNormalOrInactiveObjects++;
    }

    private void ChangeTrashObjectToAlmostGone(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.ALMOST_GONE;
        changeSharedMaterials(objDetails, almostGoneMaterials);

        // these can be selected
        nbrNormalOrInactiveObjects++;
    }

    internal void HitObject(string name)
    {
        trashGODict[name].IsHit = true;
    }

    internal void TriggerSelection()
    {
        foreach(KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict) {
            TrashGODetails objDetails = keyValuePair.Value;
            
            // handle Hit
            if (objDetails.IsHit) {
                if (objDetails.GOMode == TrashGOMode.NORMAL || objDetails.GOMode == TrashGOMode.ALMOST_GONE)
                {
                    ChangeTrashObjectToSelected(objDetails);
                }
            } else {
                if (objDetails.GOMode == TrashGOMode.SELECTED)
                {
                    if (objDetails.PreviousGOMode == TrashGOMode.NORMAL) {
                        ChangeTrashObjectToNormal(objDetails);
                    } else if (objDetails.PreviousGOMode == TrashGOMode.ALMOST_GONE) {
                        ChangeTrashObjectToAlmostGone(objDetails);
                    }
                }
            }

            // reset booleans
            objDetails.IsHit = false;
        }
    }
    internal bool TriggerFading()
    {
        bool anyObjectFading = false;
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            // handle Transparency
            if (objDetails.GOMode == TrashGOMode.SELECTED) {
                ChangeTrashObjectToFading(objDetails);
                anyObjectFading = true;
            }
        }
        // trigger fading animation
        if (anyObjectFading) {
            CreateFadingMaterialsAnimation();
        }
        return anyObjectFading;
    }
    internal void TriggerInactive()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            // handle Inactive
            if (objDetails.GOMode == TrashGOMode.FADING) {
                ChangeTrashObjectToInactive(objDetails);
            }
        }
    }
    internal void TriggerReappearing()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            if (objDetails.GOMode == TrashGOMode.FADING) {
                // increase object scale
                objDetails.GOMode = TrashGOMode.REAPPEARING;
                objDetails.GObject.transform.localScale *= notConsciousScaleIncrease;
                // to stop increasing indefinitively - max
                if (objDetails.GObject.transform.localScale.x > objDetails.MaxGOScale.x) {
                    objDetails.GObject.transform.localScale = objDetails.MaxGOScale;
                }
            }
        }
        CreateReappearingMaterialsAnimation();
    }
    internal void TriggerBackToNormal(TrashGOMode trashMode)
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            if (objDetails.GOMode == trashMode) {
                ChangeTrashObjectToNormal(objDetails);
            }
        }
    }

    private void TriggerAlmostGone()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            if (objDetails.GOMode == TrashGOMode.FADING)
            {
                ChangeTrashObjectToAlmostGone(objDetails);
            }
        }
    }

    internal bool anyObjectSelected() {
        return nbrNormalOrInactiveObjects != trashGODict.Count;
    }
}
