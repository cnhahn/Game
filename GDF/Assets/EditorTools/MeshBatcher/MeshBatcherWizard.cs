using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshBatcherWizard : ScriptableWizard
{
    public bool destroyOriginalObjects = true;
    public bool keepOriginalObjectReferences = true;

    public GameObject parentGameObject = null;

    public bool combineByGrid;
    public MeshBatcherGridType gridType;
    public float gridSize;

    public bool debug;

    [MenuItem("Tools/TylerCode/Mesh Batcher")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<MeshBatcherWizard>("Combine objects into a single mesh", "Uncombine Selected", "Combine Selected");
    }

    void OnWizardCreate()
    {
        Debug.LogWarning("Not yet implemented, sorry.");
    }

    void OnWizardUpdate()
    {
        helpString = "Will combine the meshes, textures and UVs of objects with the same mesh. Useful if you are using building kits with a lot of parts. \n\nWarning: This process is destructive (for now)";
    }

    void OnWizardOtherButton()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Editable);
        List<GameObject> gameObjects = new List<GameObject>();

        foreach(Transform t in transforms)
        {
            gameObjects.Add(t.gameObject);
        }

        MeshBatcher.CombineMeshes(gameObjects.ToArray(), destroyOriginalObjects, keepOriginalObjectReferences, combineByGrid, gridType, gridSize, parentGameObject);
    }
}
