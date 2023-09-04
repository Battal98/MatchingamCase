using UnityEngine.Events;

namespace Runtime.Abstactions.Commands
{
    public abstract class Command : ICommand
    {
        private UnityAction _action;

        public Command(UnityAction action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action?.Invoke();
        }
    }
}
