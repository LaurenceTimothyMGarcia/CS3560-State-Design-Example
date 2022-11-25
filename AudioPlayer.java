//EXAMPLE PROVIDED BY https://refactoring.guru/design-patterns/state 

public class AudioPlayer
{

}

abstract class State
{
    protected AudioPlayer player;

    abstract void clickLock();
    abstract void clickPlay();
    abstract void clickNext();
    abstract void clickPrevious();
}

class LockedState extends State
{
    void clickLock()
    {
        if (player.isPlaying)
        {
            player.changeState(new PlayingState(player));
        }
        else
        {
            player.changeState(new ReadyState(player));
        }
    }

    void clickPlay()
    {
        //LOCKED
    }

    void clickNext()
    {
        //LOCKED
    }

    void clickPrevious()
    {
        //LOCKED
    }
}

class ReadyState extends State 
{
    void clickLock()
    {
        player.changeState(new LockedState(player));
    }

    void clickPlay()
    {
        player.startPlayback();
        player.changeState(new PlayingState(player));
    }

    void clickNext()
    {
        player.nextSong();
    }

    void clickPrevious()
    {
        player.previousSong();
    }
}

class PlayState extends State 
{
    void clickLock()
    {
        player.changeState(new LockedState(player));
    }

    void clickPlay()
    {
        player.stopPlayback();
        player.changeState(new ReadyState(player));
    }

    void clickNext()
    {
        if (event.doubleClick)
        {
            player.nextSong();
        }
        else
        {
            player.fastForward(5)
        }
    }

    void clickPrevious()
    {
        if (event.doubleClick)
        {
            player.previousSong();
        }
        else
        {
            player.rewind(5)
        }
    }
}