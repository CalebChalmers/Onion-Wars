using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerInteraction : NetworkBehaviour
{
    [Header("References")]
    public GameObject viewModel;
    public GameObject onionPrefab;
    public Material highlightMaterial;
    public LayerMask selectLayerMask;
    public Transform lookPoint;

    [Header("Interaction")]
    public float interactDistance;
    public float throwForce;
    public int maxCarryCount = 3;

    private Animator animator;
    private Player player;

    private GameObject selected = null;
    private int selectableLayer;
    //private int highlightLayer;
    private float defaultHighlightAlpha;

    [SyncVar (hook = "OnCarryCountChanged")]
    private int carryCount = 0;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();

        selectableLayer = LayerMask.NameToLayer("Selectable");
        //highlightLayer = LayerMask.NameToLayer("Highlight");
        defaultHighlightAlpha = highlightMaterial.color.a;
    }
	
	void Update ()
    {
        if (!isLocalPlayer) return;
        if (GameManager.instance.State != GameState.Started) return;

        Ray ray = new Ray(player.head.position, player.head.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject obj = hit.collider.gameObject;
            
            // Depth of Field
            //lookPoint.position = Vector3.Lerp(lookPoint.position, hit.point, 10f * Time.deltaTime);

            if (hit.distance <= interactDistance && selectLayerMask == (selectLayerMask | (1 << obj.layer)))
            {
                if (selected != obj)
                {
                    Deselect();

                    if (obj.layer == selectableLayer)
                    {
                        Select(obj);
                    }
                }
            }
            else
            {
                Deselect();
            }
        }
        else
        {
            Deselect();
        }

        Color color = highlightMaterial.color;
        color.a = Mathf.Lerp(color.a, defaultHighlightAlpha, 30f * Time.deltaTime);
        highlightMaterial.color = color;

        if (CursorHelper.CursorLocked)
        {
            if (Input.GetKeyDown(KeyCode.E)) // Pickup
            {
                Pickup();
            }

            if (Input.GetMouseButtonDown(0)) // Throw
            {
                Throw();
            }
        }

        Debug.DrawRay(player.head.position, player.head.forward * interactDistance);
    }

    #region Interaction

    private void OnCarryCountChanged(int newValue)
    {
        carryCount = newValue;

        if (carryCount == 0)
        {
            viewModel.SetActive(false);
        }
        else
        {
            animator.SetTrigger("pickup");
            viewModel.SetActive(true);
        }
    }

    private void Pickup()
    {
        if (carryCount == maxCarryCount) return;
        if (selected == null) return;

        bool isOnionCrate = selected.CompareTag("Onion Crate");

        if (selected.CompareTag("Onion") || isOnionCrate)
        {
            CmdPickup(selected, isOnionCrate);
        }
    }

    [Command]
    private void CmdPickup(GameObject obj, bool isOnionCrate)
    {
        if (carryCount == maxCarryCount) return;
        carryCount++;

        if(!isOnionCrate)
        {
            NetworkServer.Destroy(obj);
        }
    }

    private void Throw()
    {
        if (carryCount == 0) return;
        CmdThrow();
    }

    [Command]
    private void CmdThrow()
    {
        if (carryCount == 0) return;
        carryCount--;

        GameObject clone = Instantiate(onionPrefab, viewModel.transform.position, viewModel.transform.rotation);
        Rigidbody rigidbody = clone.GetComponent<Rigidbody>();
        rigidbody.AddForce((Quaternion.AngleAxis(-5f, player.head.right) * player.head.forward) * throwForce, ForceMode.Impulse);
        rigidbody.AddTorque(Random.insideUnitSphere * 10f);
        clone.GetComponent<Throwable>().Throw(player);
        NetworkServer.Spawn(clone);
    }
    #endregion

    #region Selection
    public void Select(GameObject obj)
    {
        selected = obj;

        Color color = highlightMaterial.color;
        color.a = 0f;
        highlightMaterial.color = color;

        /*Selectable[] selectables = selected.GetComponents<Selectable>();
        foreach (Selectable selectable in selectables)
        {
            selectable.Select();
        }*/

        //selected.layer = highlightLayer;
        MeshRenderer[] renderers = selected.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer != null)
            {
                List<Material> mats = renderer.sharedMaterials.ToList();
                mats.Add(highlightMaterial);
                renderer.sharedMaterials = mats.ToArray();
            }
        }
    }

    public void Deselect()
    {
        if (selected == null) return;

        //selected.layer = selectableLayer;

        MeshRenderer[] renderers = selected.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer renderer in renderers)
        {
            if (renderer != null)
            {
                List<Material> mats = renderer.sharedMaterials.ToList();
                mats.Remove(highlightMaterial);
                renderer.sharedMaterials = mats.ToArray();
            }
        }

        /*Selectable[] selectables = selected.GetComponents<Selectable>();
        foreach (Selectable selectable in selectables)
        {
            selectable.Deselect();
        }*/

        selected = null;
    }
    #endregion
}
