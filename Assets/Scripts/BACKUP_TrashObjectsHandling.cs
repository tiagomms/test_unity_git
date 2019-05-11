using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BACKUP_TrashObjectsHandling : MonoBehaviour
{

    // PERFORMANCE IMPROVEMENTS: change matIndexes to arrays of each material
    private const string TRASH_MATERIALS_PATH = "Materials/Shared/TrashMaterials/";
    public enum TrashGOMode
    {
        NORMAL, SELECTED, FADING, INACTIVE
    }
    public class TrashGODetails
    {
        private int[] matIndexes;
        private Material[] normalGOMaterials;
        private Material[] selectedGOMaterials;
        private Material[] fadingGOMaterials;
        private GameObject gObject;
        private Renderer goRender;
        private TrashGOMode goMode = TrashGOMode.NORMAL;
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

        public Material[] NormalGOMaterials
        {
            get
            {
                return normalGOMaterials;
            }

            set
            {
                normalGOMaterials = value;
            }
        }

        public Material[] SelectedGOMaterials
        {
            get
            {
                return selectedGOMaterials;
            }

            set
            {
                selectedGOMaterials = value;
            }
        }

        public Material[] FadingGOMaterials
        {
            get
            {
                return fadingGOMaterials;
            }

            set
            {
                fadingGOMaterials = value;
            }
        }
    }


    public Dictionary<string, TrashGODetails> trashGODict = new Dictionary<string, TrashGODetails>();
    public Material[] normalMaterials;
    public Material[] selectedMaterials;
    public Material[] fadingMaterials;
    // utility if all objects are normal and disabled, no need to run the update function
    private int nbrNormalOrInactiveObjects = 0;
    private float beforeFadingRimPower = 3.0f;

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
        // setup materials - load them from their respective folders
        normalMaterials = Resources.LoadAll<Material>(TRASH_MATERIALS_PATH + "0_NORMAL/");
        selectedMaterials = Resources.LoadAll<Material>(TRASH_MATERIALS_PATH + "1_SELECTED/");
        fadingMaterials = Resources.LoadAll<Material>(TRASH_MATERIALS_PATH + "2_FADING/");

        GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("objectsToClean");
        foreach (GameObject obj in trashObjects)
        {
            TrashGODetails trashGODetails = new TrashGODetails();
            Renderer rend = obj.GetComponent<Renderer>();
            int[] matIndexes = new int[rend.sharedMaterials.Length];
            int curMatIndex = 0;
            int i = 0;

            while (curMatIndex < matIndexes.Length)
            {
                if (rend.sharedMaterials[curMatIndex].name.Contains(normalMaterials[i].name))
                {
                    matIndexes[curMatIndex] = i;
                    curMatIndex++;

                    i = -1;
                }
                i++;
            }

            trashGODetails.GORender = rend;
            trashGODetails.MatIndexes = matIndexes;
            trashGODetails.GObject = obj;

            trashGODict.Add(obj.name, trashGODetails);
        }
        nbrNormalOrInactiveObjects = trashGODict.Count;
    }


    private void Start()
    {
        SelectedMaterialsAnimation_Part1();
    }

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
        // fadingAnimationId = LeanTween.value(gameObject, 1f, 0f, 3f)
        //     .setEase(LeanTweenType.easeInQuad)
        //     .setOnUpdate((System.Action<float>)FadingMaterialsAnimationUpdate)
        //     .setOnComplete(FadingMaterialsAnimationComplete).id;

        // fadingAnimationLT = LeanTween.descr(fadingAnimationId);
        // LeanTween.pause(fadingAnimationId);

        // before fading animation, get the rim power of all objects
        beforeFadingRimPower = selectedMaterials[0].GetFloat("_RimPower");

        LeanTween.value(gameObject, 1f, 0f, 3f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)FadingMaterialsAnimationUpdate)
            .setOnComplete(FadingMaterialsAnimationComplete);

    }

    private void FadingMaterialsAnimationComplete()
    {
        // trigger fading objects to inactive
        TriggerInactive();
        // reset fading materials - set alpha to 1
        FadingMaterialsAnimationUpdate(1f);
    }

    private void FadingMaterialsAnimationUpdate(float value)
    {
        foreach (Material fMat in fadingMaterials)
        {
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

    private void changeSharedMaterials(TrashGODetails objDetails, Material[] materialsArray)
    {
        Material[] newSharedMaterials = new Material[objDetails.MatIndexes.Length];
        for (int i = 0; i < objDetails.MatIndexes.Length; i++)
        {
            newSharedMaterials[i] = materialsArray[objDetails.MatIndexes[i]];
        }
        objDetails.GORender.sharedMaterials = newSharedMaterials;
    }

    private void ChangeTrashObjectToNormal(TrashGODetails objDetails)
    {
        objDetails.GOMode = TrashGOMode.NORMAL;
        changeSharedMaterials(objDetails, normalMaterials);

        // increase Normal Objects;
        nbrNormalOrInactiveObjects++;
    }

    private void ChangeTrashObjectToSelected(TrashGODetails objDetails)
    {
        objDetails.GOMode = TrashGOMode.SELECTED;
        changeSharedMaterials(objDetails, selectedMaterials);

        // decrease normal objects;
        nbrNormalOrInactiveObjects--;
    }
    private void ChangeTrashObjectToFading(TrashGODetails objDetails)
    {
        objDetails.GOMode = TrashGOMode.FADING;
        changeSharedMaterials(objDetails, fadingMaterials);
    }

    private void ChangeTrashObjectToInactive(TrashGODetails objDetails)
    {
        objDetails.GObject.SetActive(false);

        changeSharedMaterials(objDetails, normalMaterials);
        // increase Normal Objects;
        nbrNormalOrInactiveObjects++;
    }

    internal void HitObject(string name)
    {
        trashGODict[name].IsHit = true;
    }

    internal void TriggerSelection()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;

            // handle Hit
            if (objDetails.IsHit)
            {
                if (objDetails.GOMode == TrashGOMode.NORMAL)
                {
                    ChangeTrashObjectToSelected(objDetails);
                }
            }
            else
            {
                if (objDetails.GOMode == TrashGOMode.SELECTED)
                {
                    ChangeTrashObjectToNormal(objDetails);
                }
            }

            // reset booleans
            objDetails.IsHit = false;
        }
    }
    internal void TriggerFading()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            // handle Transparency
            if (objDetails.GOMode == TrashGOMode.SELECTED)
            {
                ChangeTrashObjectToFading(objDetails);
            }
        }
        // trigger fading animation
        CreateFadingMaterialsAnimation();
    }
    internal void TriggerInactive()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            // handle Inactive
            if (objDetails.GOMode == TrashGOMode.FADING)
            {
                ChangeTrashObjectToInactive(objDetails);
            }
        }
    }
    internal void TriggerBackToNormal()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            // handle Inactive
            if (objDetails.GOMode == TrashGOMode.INACTIVE)
            {
                ChangeTrashObjectToNormal(objDetails);
            }
        }
    }

    internal bool anyObjectSelected()
    {
        return nbrNormalOrInactiveObjects != trashGODict.Count;
    }
}
