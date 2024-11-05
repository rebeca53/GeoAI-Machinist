using System.Collections;
using System.Collections.Generic;
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

        if (address.Contains("HerbaceousVegetation"))
        {
            return "HerbaceousVegetation";
        }

        if (address.Contains("Highway"))
        {
            return "Highway";
        }

        if (address.Contains("Industrial"))
        {
            return "Industrial";
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
        Debug.Log("On load done. " + address);
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = operation.Result;
        }
        else
        {
            Debug.LogError($"Asset for {address} failed to load.");
        }
    }
}