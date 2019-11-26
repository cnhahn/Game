using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshBatcherParent : MonoBehaviour
{
    public GameObject go;
    public List<MeshBatcherParent> SubParents = new List<MeshBatcherParent>();
    public List<MeshBatcherObject> RuntimeMeshBatcherObjects = new List<MeshBatcherObject>();

    public MeshBatcherParent(string goName)
    {
        go = new GameObject("name");
    }

    public MeshBatcherParent(GameObject gameObject)
    {
        go = gameObject;
    }

    public void CombineMeshes(
        GameObject[] gameObjects, bool destroyOriginalObjects, bool combineByGrid = false)
    {
        //Debug.Log(gameObjects.Length);
        foreach (var gameObject in gameObjects)
        {
            //Debug.Log(gameObject);
            if (gameObject == null)
                continue;

            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) continue;
            if (gameObject.GetComponentsInChildren<Renderer>()[0].isPartOfStaticBatch) continue;

            var meshObject = new MeshBatcherObject
            {
                GameObject = gameObject,
                OriginalParent = gameObject.transform.parent,
                RmbParent = this
            };
            RuntimeMeshBatcherObjects.Add(meshObject);

            MeshBatcher.SetParent(gameObject, go);
            //Debug.Log(go + " parented.");
        }

        var reflectionProbeUsages = new Dictionary<Material, ReflectionProbeUsage>();
        var lightProbeUsages = new Dictionary<Material, LightProbeUsage>();
        var rendererLayerIDs = new Dictionary<Material, int>();

        Matrix4x4 myTransform = go.transform.worldToLocalMatrix;
        var combineInstanceListsPerMaterial = new Dictionary<Material, List<List<CombineInstance>>>();
        MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            foreach (Material material in meshRenderer.sharedMaterials)
            {
                if (material != null)
                {
                    reflectionProbeUsages[material] = meshRenderer.reflectionProbeUsage;
                    lightProbeUsages[material] = meshRenderer.lightProbeUsage;
                    rendererLayerIDs[material] = meshRenderer.sortingLayerID;
                    if (!combineInstanceListsPerMaterial.ContainsKey(material))
                    {
                        combineInstanceListsPerMaterial.Add(material, new List<List<CombineInstance>>());
                    }
                }
            }
        }

        var vertexCountPerMaterial = new Dictionary<Material, int>();

        MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter filter in meshFilters)
        {
            if (filter.sharedMesh == null) continue;
            if (filter.sharedMesh.subMeshCount > 1)
            {
                var meshRenderer = filter.GetComponent<MeshRenderer>();
                for (int i = 0; i < meshRenderer.sharedMaterials.Count(); i++)
                {
                    var ci = new CombineInstance { mesh = filter.sharedMesh, subMeshIndex = i, transform = myTransform * filter.transform.localToWorldMatrix };
                    Material m = meshRenderer.sharedMaterials[i];
                    int vertexCount = ci.mesh.vertexCount;
                    if (!vertexCountPerMaterial.ContainsKey(m) || vertexCountPerMaterial[m] + vertexCount > 65536)
                    {
                        vertexCountPerMaterial[m] = 0;
                        combineInstanceListsPerMaterial[m].Add(new List<CombineInstance>());
                    }
                    int ciListCount = combineInstanceListsPerMaterial[m].Count;
                    List<CombineInstance> currentCiList = combineInstanceListsPerMaterial[m][ciListCount - 1];
                    vertexCountPerMaterial[m] += vertexCount;
                    currentCiList.Add(ci);
                }
            }
            else
            {
                var ci = new CombineInstance { mesh = filter.sharedMesh, transform = myTransform * filter.transform.localToWorldMatrix };
                Material m = filter.GetComponent<Renderer>().sharedMaterial;
                int vertexCount = ci.mesh.vertexCount;
                if (!vertexCountPerMaterial.ContainsKey(m) || vertexCountPerMaterial[m] + vertexCount > 65536)
                {
                    vertexCountPerMaterial[m] = 0;
                    combineInstanceListsPerMaterial[m].Add(new List<CombineInstance>());
                }
                int ciListCount = combineInstanceListsPerMaterial[m].Count;
                List<CombineInstance> currentCiList = combineInstanceListsPerMaterial[m][ciListCount - 1];
                vertexCountPerMaterial[m] += vertexCount;
                currentCiList.Add(ci);
            }
            filter.GetComponent<Renderer>().enabled = false;
        }

        foreach (Material m in combineInstanceListsPerMaterial.Keys)
        {
            for (int index = 0; index < combineInstanceListsPerMaterial[m].Count; index++)
            {
                List<CombineInstance> ciList = combineInstanceListsPerMaterial[m][index];
                var staticMesh = new GameObject("CombinedMesh_" + m.name + "_" + index);
                if (MeshBatcher.Debug)
                {
                    Debug.Log("Runtime Mesh Batcher: Created batch " + m.name + "_" + index);
                }
                MeshBatcher.SetParent(staticMesh, go);
                Transform staticMeshTransform = staticMesh.transform;
                staticMeshTransform.localPosition = Vector3.zero;
                staticMeshTransform.localRotation = Quaternion.identity;
                staticMeshTransform.localScale = Vector3.one;

                var filter = staticMesh.AddComponent<MeshFilter>();
                filter.mesh.CombineMeshes(ciList.ToArray(), true, true);

                var renderer = staticMesh.AddComponent<MeshRenderer>();
                renderer.material = m;
                renderer.reflectionProbeUsage = reflectionProbeUsages[m];
                renderer.lightProbeUsage = lightProbeUsages[m];
                renderer.sortingLayerID = rendererLayerIDs[m];
                if (MeshBatcher.Debug)
                {
                    Debug.Log("Runtime Mesh Batcher: Batch " + staticMesh.name + " reflection probe usage set to " + reflectionProbeUsages[m]);
                    Debug.Log("Runtime Mesh Batcher: Batch " + staticMesh.name + " light probe usage set to " + lightProbeUsages[m]);
                    Debug.Log("Runtime Mesh Batcher: Batch " + staticMesh.name + " renderer layer set to " + rendererLayerIDs[m]);
                }
            }
        }

        if (destroyOriginalObjects) for (int i = 0; i < gameObjects.Length; i++) Object.DestroyImmediate(gameObjects[i]);
        else
        {
            foreach (var rmbObject in RuntimeMeshBatcherObjects)
            {
                rmbObject.SetState(false);
                rmbObject.SetOriginalParent();
            }
            //foreach (MeshRenderer meshRenderer in meshRenderers) meshRenderer.enabled = false;
        }
    }

    public void UncombineMeshes()
    {
        if (SubParents.Count > 0)
        {
            foreach (var runtimeMeshBatcherParent in SubParents)
            {
                runtimeMeshBatcherParent.UncombineMeshes();
            }
            return;
        }

        foreach (var rmbObject in RuntimeMeshBatcherObjects)
        {
            if (rmbObject.GameObject == null) continue;
            rmbObject.SetState(true);
            rmbObject.SetOriginalParent();
        }

    }

    public void AddSubParent(MeshBatcherParent subParent)
    {
        SubParents.Add(subParent);
        MeshBatcher.SetParent(subParent.go, go);
    }
}
