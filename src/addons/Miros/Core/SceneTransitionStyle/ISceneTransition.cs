using System.Threading.Tasks;

public interface ISceneTransition
{
    Task TransitionIn();
    Task TransitionOut();
}