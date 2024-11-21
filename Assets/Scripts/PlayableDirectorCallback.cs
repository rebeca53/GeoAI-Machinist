using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableDirectorCallback : MonoBehaviour
{
    public PlayableDirector director;
    public PlayerController Player;
    public NonPlayerCharacter NPC;

    void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");

            Player.TurnRight();
            NPC.Speak();
        }


    }

    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }
}
