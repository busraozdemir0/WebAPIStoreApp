namespace Entities.Exceptions
{
    public sealed class BookNotFoundException : NotFoundException  // sealed ile başka bir class tarafından kalıtılması mümkün olmayacak
    {
        public BookNotFoundException(int id) : base($"The book with id : {id} could not found.")
        {
        }
    }
}
