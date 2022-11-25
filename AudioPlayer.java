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