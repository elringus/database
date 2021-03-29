namespace Database.EntityFramework
{
    public class EntityFrameworkTransaction : ITransaction
    {
        public void WaitForCompletion () { }
    }
}
