namespace Enmarcha.SharePoint.Abstract.Interfaces.Artefacts
{
    public interface IContentType
    {
        bool Create();
        bool Create(string id);
        
        bool Delete();
        bool Exist();
        bool AddColumn(string name);

        bool RemoveColumn(string name);
    }
}
