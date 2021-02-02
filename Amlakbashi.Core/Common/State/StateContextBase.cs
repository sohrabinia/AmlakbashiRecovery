namespace Amlakbashi.Core.Common.State
{
    public abstract class StateContextBase<T>
    {
        protected T data;
        protected virtual StateBase<T> state { get; set; }
        public StateContextBase(T data,StateBase<T> state)
        {
            this.data = data;
            TransitionTo(state);
        }

        public void TransitionTo(StateBase<T> state)
        {
            this.state = state;
        }
    }
}
