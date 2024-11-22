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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start HintBalloon");
        spaceKey = transform.Find("Space");
        arrowRightKey = transform.Find("ArrowRight");
        SetArrowRightKey();
        Place();
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
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(hintedKey))
        {
            Debug.Log("key pressed");
            Hide();
        }
    }

}
