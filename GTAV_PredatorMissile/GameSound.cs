using System;
using GTA;
using GTA.Native;

public class GameSound
{
    private string soundSet;
    private string sound;
    private int soundID;

    public GameSound(string sound, string soundSet)
    {
        this.sound = sound;
        this.soundSet = soundSet;
        this.soundID = Function.Call<int>(Hash.GET_SOUND_ID);
    }

    public void Play(Entity ent)
    {
        Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, soundID, sound, ent.Handle, soundSet, 0, 0);
    }

    public void Destroy()
    {
        Function.Call(Hash.STOP_SOUND, soundID);
        Function.Call(Hash.RELEASE_SOUND_ID, soundID);
    }
}
