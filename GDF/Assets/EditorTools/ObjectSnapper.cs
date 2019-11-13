// Creates a simple wizard that lets you create a Light GameObject
// or if the user clicks in "Apply", it will set the color of the currently
// object selected to red

using UnityEditor;
using UnityEngine;

public class ObjectSnapper : ScriptableWizard
{
    public float snappingRange = 1;
    public bool roundUpOnly = false;

    [MenuItem("Tools/Object Snapper")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ObjectSnapper>("Snap Objects to Units", "Snap All", "Snap Selected");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
        //GameObject.Find();
    }

    void OnWizardUpdate()
    {
        helpString = "Will snap objects to full whole number of meters";
    }

    // When the user presses the "Apply" button OnWizardOtherButton is called.
    void OnWizardOtherButton()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Editable);

        if(transforms != null)
        {
            foreach(Transform t in transforms)
            {
                t.position = new Vector3(Mathf.Round(t.position.x), Mathf.Round(t.position.y), Mathf.Round(t.position.z));
            }
        }
    }
}