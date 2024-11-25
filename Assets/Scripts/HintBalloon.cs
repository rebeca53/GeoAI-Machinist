using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintBalloon : MonoBehaviour
{
    public GameObject target;

    public Transform spaceKey;

    public Transform arrowRightKey;

    float yOffset = 0.55f;
    KeyCode hintedKey;

    GameObject nearObject;

    public Action OnDone;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start HintBalloon");
        spaceKey = transform.Find("Space");
        arrowRightKey = transform.Find("ArrowRight");
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void Place()
    {
        if (target == null)
        {
            Debug.LogError("No Target to use as position reference");
            return;
        }

        transform.position = new(target.transform.position.x, target.transform.position.y + yOffset, target.transform.position.z);
    }

    public void SetSpaceKey()
    {
        hintedKey = KeyCode.Space;
        spaceKey.gameObject.SetActive(true);
        arrowRightKey.gameObject.SetActive(false);
    }

    public void SetArrowRightKey()
    {
        hintedKey = KeyCode.RightArrow;
        spaceKey.gameObject.SetActive(false);
        arrowRightKey.gameObject.SetActive(true);
    }

    public void Show()
    {
        Debug.Log("Show hint balloon");
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(hintedKey))
        {
            if (nearObject.CompareTag("Player"))
            {
                Debug.Log("hinted key: " + hintedKey);
                Debug.Log("key pressed");
                Hide();
                OnDone?.Invoke();
            }
            else
            {
                Debug.Log("key pressed but Player was not over me");
            }

        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        nearObject = other.gameObject;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log("On trigger stay 2d " + other.tag);
        nearObject = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("on trigger exit 2d " + other.tag);
        nearObject = null;
    }

}
