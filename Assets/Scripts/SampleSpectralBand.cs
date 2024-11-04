using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SampleSpectralBand : MonoBehaviour
{
    string address;

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
