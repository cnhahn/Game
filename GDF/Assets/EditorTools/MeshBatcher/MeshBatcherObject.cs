using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBatcherObject : MonoBehaviour
{
    public GameObject GameObject;
    public Transform OriginalParent;
    public MeshBatcherParent RmbParent;
    public GameObject State;

    public void SetState(bool state)
    {
        MeshRenderer[] meshRenderersOnObject = GameObject.GetComponents<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderersOnObject) meshRenderer.enabled = state;
        MeshRenderer[] meshRenderers = GameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers) meshRenderer.enabled = state;
    }

    public void SetOriginalParent()
    {
        if (GameObject == null) return;
        if (OriginalParent == null)
        {
            GameObject.transform.parent = null;
            return;
        }
        MeshBatcher.SetParent(GameObject, OriginalParent.gameObject);
    }
}
