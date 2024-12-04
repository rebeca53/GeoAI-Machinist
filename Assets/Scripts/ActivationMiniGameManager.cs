using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationMiniGameManager : BaseBoard
{
    // Pre-fabs

    // Instances
    public TimedDialogueBalloon timedDialogueBalloon;
    public DialogueBalloon dialogueBalloon;
    public CameraZoom cameraZoom;

    // Start is called before the first frame update
    void Start()
    {
        InitializeTilemap();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                DrawFloor(x, y);

                if (IsBorder(x, y) && !IsExit(x, y))
                {
                    DrawWall(x, y);
                }
                if (IsExit(x, y))
                {
                    DrawExit(x, y);
                }
            }
        }

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Activation 1"] = true;
        GameManager.instance.StartOverviewScene();
    }
}
