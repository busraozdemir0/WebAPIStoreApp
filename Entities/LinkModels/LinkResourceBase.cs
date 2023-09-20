namespace Entities.LinkModels
{
    public class LinkResourceBase
    {
        public LinkResourceBase()
        {

        }

        public List<Link> Links { get; set; } = new List<Link>(); // referansını ya constructor'da ya da tanımlandığı yerde aldırmalıyız
    }

}
