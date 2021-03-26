namespace Database
{
    public interface ITransaction
    {
        bool WaitForCompletion ();
    }
}
