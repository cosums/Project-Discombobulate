using TMPro;
using UnityEngine;

public class InspectObjectUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI objectNameTMP;
    [SerializeField] private TextMeshProUGUI objectDescriptionTMP;

    [Header("Input")]
    [SerializeField] private InspectObjectInput inspectInput;

    [Header("Preview Stage")]
    [SerializeField] private Camera previewCamera;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float targetSize = 1f;   // model is scaled to fit this size

    private GameObject pivot;

    public static InspectObjectUI Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }


    public void Show(LoreItemData data)
    {
        ClearModel();

        objectNameTMP.text = data.ItemName;
        objectDescriptionTMP.text = data.Description;

        if (data.ModelPrefab != null)
            SpawnModel(data.ModelPrefab);

        previewCamera.enabled = true;
        panelRoot.SetActive(true);

        UIManager.EnableCursor();
    }

    public void Hide()
    {
        ClearModel();
        previewCamera.enabled = false;
        panelRoot.SetActive(false);
        UIManager.DisableCursor();
    }

    private void SpawnModel(GameObject prefab)
    {
        int inspectLayer = LayerMask.NameToLayer("Inspect");

        pivot = new GameObject("InspectPivot");
        pivot.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);

        GameObject model = Instantiate(prefab, pivot.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = prefab.transform.rotation; // keep the prefab's original orientation

        foreach (var t in model.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = inspectLayer;

        // Bounds are measured after the rotation is applied, so centering still works
        var renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);

            model.transform.position -= bounds.center - pivot.transform.position;

            float maxExtent = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (maxExtent > 0f)
                pivot.transform.localScale = Vector3.one * (targetSize / maxExtent);
        }

        inspectInput.SetTarget(pivot.transform);
    }

    private void ClearModel()
    {
        inspectInput.SetTarget(null);
        if (pivot != null) Destroy(pivot);
    }
}