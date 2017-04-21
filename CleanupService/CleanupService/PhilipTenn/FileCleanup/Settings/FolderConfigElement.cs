using System;
using System.Configuration;

namespace PhilipTenn.FileCleanup.Settings
{
    /// <summary>
    /// Represents a single Monitored Folder Configuration Element.
    /// </summary>
    public class FolderConfigElement : ConfigurationElement
    {
        /// <summary>
        /// Default, parameter-less constructor
        /// </summary>
        public FolderConfigElement()
        {            
        }

        /// <summary>
        /// Constructor allowing name
        /// </summary>
        public FolderConfigElement(String elementName)
        {
            Name = elementName;
        }

        /// <summary>
        /// Constructor allowing name, path, cleanupDays to be specified
        /// </summary>
        /// <param name="elementName">The new name of the element</param>
        /// <param name="path">The path for the folder to be monitored.</param>
        /// <param name="cleanupDays">Number of days that a file should reach before it is deleted.</param>
        public FolderConfigElement(String elementName,
            String path, int cleanupDays)
        {
            Name = elementName;
            Path = path;
            CleanupDays = cleanupDays;
        }

        /// <summary>
        /// Constructor allowing name and path to be specified, will use default for cleanupDays 
        /// </summary>
        /// <param name="elementName">The new name of the element</param>
        /// <param name="path">The path for the folder to be monitored.</param>
        public FolderConfigElement(string elementName, String path)
        {
            Name = elementName;
            Path = path;
        }

        [ConfigurationProperty("name", 
            DefaultValue = "Folder",
            IsRequired = true, 
            IsKey = true)]
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

        [ConfigurationProperty("path",
            DefaultValue = "C:/Temp",
            IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        [ConfigurationProperty("cleanupDays",
            DefaultValue = 0,
            IsRequired = true)]
        [IntegerValidator(MinValue = 0,
            MaxValue = 365, ExcludeRange = false)]
        public int CleanupDays
        {
            get
            {
                return (int)this["cleanupDays"];
            }
            set
            {
                this["cleanupDays"] = value;
            }
        }

        [ConfigurationProperty("cleanupHours",
            DefaultValue = 0,
            IsRequired = true)]
        [IntegerValidator(MinValue = 0,
            MaxValue = 23, ExcludeRange = false)]
        public int CleanupHours
        {
            get
            {
                return (int)this["cleanupHours"];
            }
            set
            {
                this["cleanupHours"] = value;
            }
        }

        [ConfigurationProperty("deleteSubFolders",
            DefaultValue = "false",
            IsRequired = false)]
        public bool DeleteSubFolders
        {
            get
            {
                return (bool) this["deleteSubFolders"];
            }
            set
            {
                this["deleteSubFolders"] = value;
            }
        }

    }
}