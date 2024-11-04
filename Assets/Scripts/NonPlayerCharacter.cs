using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("On trigger stay 2d " + other.tag);
        if (other.CompareTag("Player"))
        {
            UIHandler.Instance.DisplayIntroduction();
        }
    }

    public void Spawn(OverviewBoardManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

    public void Spawn(CommandCenterBoardManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

    public void Spawn(InputMiniGameManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

}
