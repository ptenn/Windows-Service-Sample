using System;
using System.Configuration;

namespace PhilipTenn.FileCleanup.Settings
{
    public class FoldersCollection : ConfigurationElementCollection
    {

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderConfigElement();
        }


        protected override 
            ConfigurationElement CreateNewElement(
            string elementName)
        {
            return new FolderConfigElement(elementName);
        }


        protected override Object 
            GetElementKey(ConfigurationElement element)
        {
            return ((FolderConfigElement)element).Name;
        }


        public new string AddElementName
        {
            get { return base.AddElementName; }
            set { base.AddElementName = value; }
        }

        public new string ClearElementName
        {
            get { return base.ClearElementName; }
            set { base.AddElementName = value; }
        }

        public new string RemoveElementName
        {
            get { return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }


        public FolderConfigElement this[int index]
        {
            get
            {
                return (FolderConfigElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public FolderConfigElement this[string Name]
        {
            get
            {
                return (FolderConfigElement)BaseGet(Name);
            }
        }

        public int IndexOf(FolderConfigElement folder)
        {
            return BaseIndexOf(folder);
        }

        public void Add(FolderConfigElement folder)
        {
            BaseAdd(folder);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(FolderConfigElement folder)
        {
            if (BaseIndexOf(folder) >= 0)
                BaseRemove(folder.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}