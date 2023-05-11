using UnityEngine.Events;

namespace ReiBrary.Input
{
    public interface IGameplayInputReader
    {
        public event UnityAction MenuPauseEvent;

        public void EnableGameplayInput();
    }
}