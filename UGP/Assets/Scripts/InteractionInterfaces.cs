namespace UGP
{
    public interface IInteractor
    {
        void Interaction_Set(IInteractable interactable);
        void Interaction_Release();
    }

    public interface IInteractable
    {
        void Interact(object token);
    }
}
