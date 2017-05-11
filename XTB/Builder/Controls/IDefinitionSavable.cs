// PROJECT : MsCrmTools.SiteMapEditor
// This project was developed by Tanguy Touzard
// CODEPLEX: http://xrmtoolbox.codeplex.com
// BLOG: http://mscrmtools.blogspot.com
namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public interface IDefinitionSavable
    {
        void Save();

        string ControlsChecksum();
    }
}
