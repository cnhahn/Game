using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class MeshBatcher : MonoBehaviour
{
    public static bool Debug;

    public static Dictionary<GameObject, MeshBatcherParent> meshParents;
    public static GameObject rmbContainerGameObject;

    public static GameObject CombineMeshes(
        bool processObjectsByTag = false,
        string rmbTag = null,
        bool processObjectsByLayer = false,
        int rmbLayer = 0,
        bool destroyOriginalObjects = false,
        bool keepOriginalObjectReferences = false,
        bool combineByGrid = false,
        MeshBatcherGridType gridType = MeshBatcherGridType.Grid2D,
        float gridSize = 0,
        GameObject parentGameObject = null)
    {

        // Build Objects List
        var objectsToBatch = new List<GameObject>();

        // Add objects by tag
        if (processObjectsByTag && rmbTag != null) objectsToBatch.AddRange(GameObject.FindGameObjectsWithTag(rmbTag));

        // Add objects by layer
        if (processObjectsByLayer)
        {
            GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();
            for (int i = 0; i < allGameObjects.Length; i++)
            {
                GameObject go = allGameObjects[i];
                if (go.layer == rmbLayer) objectsToBatch.Add(go);
            }
        }

        return CombineMeshes(objectsToBatch.ToArray(), destroyOriginalObjects, keepOriginalObjectReferences, combineByGrid, gridType, gridSize, parentGameObject);
    }

    public static GameObject CombineMeshes(
        GameObject[] objectsToBatch, bool destroyOriginalObjects = false, bool keepOriginalObjectReferences = false, bool combineByGrid = false,
        MeshBatcherGridType gridType = MeshBatcherGridType.Grid2D,
        float gridSize = 0, GameObject parentGameObject = null)
    {
        if (objectsToBatch == null || objectsToBatch.Length == 0)
        {
            UnityEngine.Debug.LogWarning("Runtime Mesh Batcher warning: no objects found to be combined.");
            return null;
        }

        if (combineByGrid && gridSize <= 0)
        {
            UnityEngine.Debug.LogWarning("Runtime Mesh Batcher warning: Grid Size must be superior to 0. Continuing batching without grid.");
            combineByGrid = false;
        }

        if (meshParents == null) meshParents = new Dictionary<GameObject, MeshBatcherParent>();

        if (parentGameObject != null && meshParents.ContainsKey(parentGameObject))
        {
            UnityEngine.Debug.LogWarning("Runtime Mesh Batcher warning: GameObject already used as a parent.");
            return null;
        }


        var rmbParent = new MeshBatcherParent(parentGameObject);
        meshParents[rmbParent.go] = rmbParent;
        if (Debug)
        {
            UnityEngine.Debug.Log("Runtime Mesh Batcher: Starting batch process under parent " + rmbParent.go);

        }

        if (rmbContainerGameObject == null) rmbContainerGameObject = new GameObject("RuntimeMeshBatcherContainer");
        SetParent(rmbParent.go, rmbContainerGameObject);


        if (combineByGrid)
        {
            var objectsByCell = new Dictionary<string, List<GameObject>>();
            switch (gridType)
            {
                case MeshBatcherGridType.Grid2D:
                    foreach (var gameObject in objectsToBatch)
                    {
                        var position = gameObject.transform.position;
                        var xIndex = GetGridIndex(position.x, gridSize);
                        var zIndex = GetGridIndex(position.z, gridSize);
                        string key = xIndex + "_" + zIndex;
                        if (!objectsByCell.ContainsKey(key))
                        {
                            objectsByCell[key] = new List<GameObject>();
                        }
                        objectsByCell[key].Add(gameObject);
                    }
                    break;
                case MeshBatcherGridType.Grid3D:
                    foreach (var gameObject in objectsToBatch)
                    {
                        var position = gameObject.transform.position;
                        var xIndex = GetGridIndex(position.x, gridSize);
                        var yIndex = GetGridIndex(position.y, gridSize);
                        var zIndex = GetGridIndex(position.z, gridSize);
                        string key = xIndex + "_" + yIndex + "_" + zIndex;
                        if (!objectsByCell.ContainsKey(key))
                        {
                            objectsByCell[key] = new List<GameObject>();
                        }
                        objectsByCell[key].Add(gameObject);
                    }
                    break;
            }

            foreach (var gameObjects in objectsByCell.Values)
            {
                var SubParent = new MeshBatcherParent("SubParent");
                SubParent.CombineMeshes(gameObjects.ToArray(), destroyOriginalObjects, true);
                rmbParent.AddSubParent(SubParent);
            }
        }
        else
        {
            rmbParent.CombineMeshes(objectsToBatch, destroyOriginalObjects);
        }

        return rmbParent.go;
    }

    private static int GetGridIndex(float position, float gridSize)
    {
        return (int)Mathf.Floor(position / gridSize);
    }

    public static void UncombineMeshes(GameObject meshParent)
    {
        if (meshParent == null)
        {
            UnityEngine.Debug.LogWarning("Mesh Batcher warning: Calling UncombineMeshes with null parent GameObject.");
            return;
        }

        if (!meshParents.ContainsKey(meshParent))
        {
            UnityEngine.Debug.LogWarning("Mesh Batcher warning: Calling UncombineMeshes with undefined parent GameObject.");
            return;
        }

        meshParents[meshParent].UncombineMeshes();
        meshParents[meshParent] = null;
        meshParents.Remove(meshParent);

        Object.Destroy(meshParent);
    }

    public static void SetParent(GameObject gameObject, GameObject parent)
    {
        SetParent(gameObject.transform, parent.transform);
    }

    public static void SetParent(Transform gameObjectTransform, Transform parentTransform)
    {
        gameObjectTransform.parent = parentTransform;
    }
}

public enum MeshBatcherGridType
{
    Grid2D,
    Grid3D,
}