using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SampleSpectralBand : MonoBehaviour
{
    string address;
    private BoxCollider2D boxCollider;

    public string GetClass()
    {
        if (address.Contains("AnnualCrop"))
        {
            return "AnnualCrop";
        }

        if (address.Contains("Forest"))
        {
            return "Forest";
        }

        if (address.Contains("Residential"))
        {
            return "Residential";
        }

        if (address.Contains("Highway"))
        {
            return "Highway";
        }

        if (address.Contains("River"))
        {
            return "River";
        }
        return "";
    }

    public string GetBandType()
    {
        if (address.Contains("RedEdge"))
        {
            return "redEdge";
        }

        if (address.Contains("Red"))
        {
            return "red";
        }

        if (address.Contains("Green"))
        {
            return "green";
        }

        if (address.Contains("Blue"))
        {
            return "blue";
        }

        if (address.Contains("SWIR"))
        {
            return "swir";
        }
        return "";
    }

    public void FitInContainer()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
    }

    public void LoadSprite(string address)
    {
        this.address = address;
        Addressables.LoadAssetAsync<Sprite>(address).Completed += OnLoadDone;
    }

    // Instantiate the loaded prefab on complete
    private void OnLoadDone(AsyncOperationHandle<Sprite> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = operation.Result;
            gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"Asset for {address} failed to load.");
        }
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
}
