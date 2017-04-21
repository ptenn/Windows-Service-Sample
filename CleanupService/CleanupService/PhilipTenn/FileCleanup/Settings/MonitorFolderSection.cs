using System.Configuration;

namespace PhilipTenn.FileCleanup.Settings
{
    
    // Define a custom section containing an individual
    // element and a collection of elements.
    public class MonitorFolderSection : ConfigurationSection
    {
        [ConfigurationProperty("name", 
            DefaultValue = "Monitor Folders",
            IsRequired = false, 
            IsKey = false)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("frequency",
            DefaultValue = "daily",
            IsRequired = false,
            IsKey = false)]
        public string Frequency
        {
            get
            {
                return (string)this["frequency"];
            }
            set
            {
                this["frequency"] = value;
            }
        }

        // Declare a collection element represented 
        // in the configuration file by the sub-section
        // <urls> <add .../> </urls> 
        // Note: the "IsDefaultCollection = false" 
        // instructs the .NET Framework to build a nested 
        // section like <urls> ...</urls>.
        [ConfigurationProperty("folders",
            IsDefaultCollection = false)]
        public FoldersCollection Folders
        {
            get
            {
                FoldersCollection urlsCollection = (FoldersCollection)base["folders"];
                return urlsCollection;
            }
        }

        protected override string SerializeSection(
            ConfigurationElement parentElement,
            string name, ConfigurationSaveMode saveMode)
        {
            string s =
                base.SerializeSection(parentElement,
                name, saveMode);
            return s;
        }

    }
}
