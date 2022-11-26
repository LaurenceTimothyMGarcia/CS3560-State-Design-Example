//EXAMPLE PROVIDED BY https://refactoring.guru/design-patterns/state 

public class AudioPlayer
{

    private State state;

    private UserInterface UI;
    private Volumne vol;
    private ArrayList playlist;
    private Song currentSong;

    public AudioPlayer()
    {
        state = new ReadyState(this);

        UI = new UserInterface();
        UI.lockButton.onClick(this.clickLock)
        UI.playButton.onClick(this.clickPlay)
        UI.nextButton.onClick(this.clickNext)
        UI.prevButton.onClick(this.clickPrevious)
    }

    public void changeState(State state)
    {
        this.state = state;
    }

    public void clickLock()
    {
        state.clickLock();
    }

    public void clickPlay()
    {
        state.clickPlay();
    }

    public void clickNext()
    {
        state.clickNext();
    }

    public void clickPrevious()
    {
        state.clickPrevious();
    }

    public void startPlayback()
    {
        //Play music
    }

    public void stopPlayback()
    {
        //Stop playing music
    }

    public void nextSong()
    {
        //Skip to next song
    }

    public void previousSong()
    {
        //Go to previous song
    }

    public void fastForward(float time)
    {
        //Move forward x amount of seconds
    }

    public void rewind(float time)
    {
        //Move backwards x amoutn of seconds
    }

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