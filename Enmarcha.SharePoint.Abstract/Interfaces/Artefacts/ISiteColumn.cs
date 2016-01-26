namespace Enmarcha.SharePoint.Abstract.Interfaces.Artefacts
{
    public interface ISiteColumn
    {
        bool Create();
        bool CreateTaxonomy(string group, string termSet, bool multivalue, bool requiered);
        bool Delete();
        bool Exist();
        bool RenameField(string oldName, string newName);
    }
}
