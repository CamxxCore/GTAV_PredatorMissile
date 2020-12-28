using System;
using System.Media;

public class ExternalSound : IDisposable
{
    private SoundPlayer player;
    public SoundPlayer Player
    {
        get { return player; }
    }

    public ExternalSound(string filePath)
    {
        player = new SoundPlayer(filePath);
        player.Load();
    }

    public ExternalSound(System.IO.Stream fileStream)
    {
        player = new SoundPlayer(fileStream);
        player.Load();
    }

    public void Play()
    {
        if (player.IsLoadCompleted)
        player.Play();
    }

    public void Dispose()
    {
        if (this != null)
        {
            player.Stop();
            player.Dispose();
        }
    }
}

