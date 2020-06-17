using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenager : MonoBehaviour
{
    public AudioSource Buttom;

    public AudioSource BrickDie;

    public AudioSource ContactBrick;

    public AudioSource Gameover;

    public AudioSource Score;


    public static SoundMenager Singlton
    {
        get;
        private set;
    }


    private void Awake()
    {
        if (Singlton == null)
            Singlton = this;
    }

    public void PlaySound(SoundGame sound)
    {

        switch (sound)
        {
            case SoundGame.Buttom:
                Buttom.Play();
                break;
            case SoundGame.BrickDie:
                BrickDie.Play();
                break;
            case SoundGame.ContactBrick:
               // ContactBrick.Play();
                break;
            case SoundGame.Gameover:
                Gameover.Play();
                break;
            case SoundGame.Score:
                Score.Play();
                break;
        }
        
    }
   
}

public enum SoundGame
{
    Buttom,
    BrickDie,
    ContactBrick,
    Gameover,
    Score
};

