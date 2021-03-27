namespace Database
{
    public interface ITransaction
    {
        void WaitForCompletion ();
    }
}
