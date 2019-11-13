using UnityEditor;
using UnityEngine;

public class ObjectSnapper : ScriptableWizard
{
    public int snappingRange = 1;
    public bool snapPosition = false;
    public bool snapRotation = false;
    public int rotationAngle = 90;

    [MenuItem("Tools/Object Snapper")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ObjectSnapper>("Snap Objects to Units", "Snap All", "Snap Selected");
    }

    void OnWizardCreate()
    {
        Debug.LogWarning("Not yet implemented, sorry.");
    }

    void OnWizardUpdate()
    {
        helpString = "Will snap objects to whole meter. Can also snap angles to make sure those are all set. \n\nWarning: Can flip objects randomly. (rare)";
    }

    void OnWizardOtherButton()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Editable);

        if(transforms != null)
        {
            foreach(Transform t in transforms)
            {
                t.position = new Vector3(Mathf.Round(t.position.x), Mathf.Round(t.position.y), Mathf.Round(t.position.z));
                
                if(snapRotation)
                {
                    Vector3 rotAngle = t.rotation.eulerAngles;

                    t.rotation = Quaternion.Euler(Mathf.Round(rotAngle.x / rotationAngle) * rotationAngle,
                        Mathf.Round(rotAngle.y / rotationAngle) * rotationAngle,
                        Mathf.Round(rotAngle.z / rotationAngle) * rotationAngle);
                }
            }
        }
    }
}