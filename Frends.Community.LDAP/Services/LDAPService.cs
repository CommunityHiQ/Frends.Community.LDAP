using Frends.Community.LDAP.Models;
using Frends.Community.LDAP.Services;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Services
{
    public class LdapService : IDisposable
    {
        private DirectoryEntry _rootEntry;

        public LdapService(LdapConnectionInfo ldapConnectionInfo)
        {
            _rootEntry = new DirectoryEntry(ldapConnectionInfo.LdapUri, ldapConnectionInfo.Username, ldapConnectionInfo.Password, ldapConnectionInfo.GetAuthenticationType());
        }

        public DirectoryEntry CreateAndSaveEntry(string name, string organizationUnit, string schemaClass, IEnumerable<EntryAttribute> attributes)
        {
            var attrDictionary = GetEntryAttributes(null, attributes);
            return CreateEntry(name, organizationUnit, schemaClass, attrDictionary);
        }

        private DirectoryEntry CreateEntry(string name, string organizationUnit, string schemaClass, Dictionary<string,object> entry)
        {
            var parent = _rootEntry;
            if(!string.IsNullOrEmpty(organizationUnit))
                parent = FindPath(_rootEntry, organizationUnit);

            DirectoryEntry childEntry = parent.Children.Add(name, schemaClass);
            childEntry = SetDirectoryEntryAttributes(childEntry, entry, false);           
            return childEntry;
        }

        public DirectoryEntry FindPath(DirectoryEntry searchRoot, string path)
        {
            try
            {
            string filter = "distinguishedname=" + path;
            using (DirectorySearcher s = new DirectorySearcher(searchRoot))
            {
                s.SearchScope = SearchScope.Subtree;
                s.ReferralChasing = ReferralChasingOption.All;
                s.Filter = filter;
                SearchResult res = s.FindOne();
                if (res == null)
                {
                    return null;
                }
                else
                {
                    return res.GetDirectoryEntry();
                }
            }
            } catch(Exception ex)
            {
                string message = "Failed finding directory entry with path '{0}' under path '{1}'." + ex.Message;
                string rootPath = searchRoot != null ? searchRoot.Path : "";
                throw new ArgumentException(string.Format(message,path, rootPath), ex);
            }
        }

        private bool SaveEntry(DirectoryEntry entry, bool saveRootEntry = true)
        {
            try
            {
                if(saveRootEntry)
                    _rootEntry.CommitChanges();
                entry.CommitChanges();
                entry.Close();
            }
            catch (DirectoryServicesCOMException ex)
            {
                throw new ArgumentException(ex.Message + " " + ex.ExtendedErrorMessage, ex);
            }
            return true;
        }

        public DirectoryEntry CreateAdUser(AdUser user)
        {
            var entryAttributes = GetEntryAttributes(user, user.OtherAttributes);
            if(!user.CN.ToUpper().StartsWith("CN="))
                user.CN = "CN=" + user.CN;

            var entry = CreateEntry(user.CN, user.OU, LdapClasses.User, entryAttributes);
            SaveEntry(entry);

            foreach (var flag in user.ADFlags)
            {
                entry.SetAccountFlag(flag.FlagType, flag.Value);
            }

            SaveEntry(entry);
            return entry;
        }

        public DirectoryEntry UpdateAdUser(AdUser user)
        {
            if (!user.CN.ToUpper().StartsWith("CN="))
                user.CN = "CN=" + user.CN;

            var entry = FindPath(_rootEntry, user.GetPath());
            var entryAttributes = GetEntryAttributes(user, user.OtherAttributes);
            SetDirectoryEntryAttributes(entry, entryAttributes, true);
            SaveEntry(entry);
            return entry;
        }

        /// <summary>
        /// Searches for the collection of the objects based on given filter. 
        /// </summary>
        /// <param name="filter">The attribute to filter the search by</param>
        /// <returns> The list of the DirectoreEntry(s) objects</returns>
         public List<DirectoryEntry> SearchObjectsByFilter(string filter)
        {
            var ret = new List<DirectoryEntry>();
            try
            {
                using (DirectorySearcher s = new DirectorySearcher(_rootEntry))
                {
                    s.SearchScope = SearchScope.Subtree;
                    s.ReferralChasing = ReferralChasingOption.All;
                    s.Filter = filter;
                    SearchResultCollection ResultCollection = s.FindAll();
       
                    if (ResultCollection == null)
                    {
                        return ret;
                    }
                    else
                    {
                        foreach (SearchResult item in ResultCollection)
                        {
                            ret.Add(item.GetDirectoryEntry());
                        }
                        return ret;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Failed finding objects with filter {0}:'." + ex.Message;
                throw new ArgumentException(string.Format(message, filter), ex);
                throw new Exception(ex.Message);
            }
        }


        #region Windows Users
        public DirectoryEntry CreateWindowsUser(WindowsUser user)
        {
            var entryAttributes = GetEntryAttributes(user, user.OtherAttributes);
            var entry = CreateEntry(user.UserName, null, LdapClasses.User, entryAttributes);
            entry.Invoke("SetPassword", new object[] { user.Password });
            entry.Invoke("Put", new object[] { "Description", user.Description });
            SaveEntry(entry, false);
            return entry;
        }

        public DirectoryEntry UpdateWindowsUser(WindowsUser user)
        {
            var entry = SearchEntry(user.UserName, LdapClasses.User);
            entry.Invoke("SetPassword", new object[] { user.Password });
            entry.Invoke("Put", new object[] { "Description", user.Description });
            SaveEntry(entry, false);
            return entry;
        }

        public bool AddAdUserToGroup(AdUser user, IEnumerable<string> groups)
        {
            var entry = FindPath(_rootEntry, user.GetPath());
            foreach (var groupName in groups)
            {
                var groupEntry = SearchEntry(groupName, LdapClasses.Group);
                if (!IsMemberOf(entry, groupEntry))
                {
                    groupEntry.Invoke(LdapMethods.Add, new object[] { entry.Path.ToString() });
                    SaveEntry(groupEntry, false);
                }
            }        
            SaveEntry(entry, false);
            return true;
        }

        public bool AddUserToGroup(WindowsUser user, IEnumerable<string> groups)
        {
            var userEntry = SearchEntry(user.UserName, LdapClasses.User);

            foreach (var groupName in groups)
            {
                var groupEntry = SearchEntry(groupName, LdapClasses.Group);
                if (!IsMemberOf(userEntry, groupEntry))
                {
                    groupEntry.Invoke(LdapMethods.Add, new object[] { userEntry.Path.ToString() });
                }
            }

            SaveEntry(userEntry, false);
            return true;
        }
        #endregion


        private static bool IsMemberOf(DirectoryEntry userEntry, DirectoryEntry groupEntry)
        {
            return (bool)groupEntry.Invoke(LdapMethods.IsMember, new object[] { userEntry.Path.ToString() });
        }

        public DirectoryEntry SearchEntry(string name, string schemaClass)
        {
            return _rootEntry.Children.Find(name, schemaClass);
        }



        private Dictionary<string, object> GetEntryAttributes(object entry, IEnumerable<EntryAttribute> attributes)
        {
            var dictionaryEntry = new Dictionary<string, Object>();

            if (entry != null)
            {
                var properties = entry.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(LdapEntryAttribute)));

                foreach (var prop in properties)
                {
                    var ldapAttribute = (LdapEntryAttribute)Attribute.GetCustomAttribute(prop, typeof(LdapEntryAttribute));
                    if (!dictionaryEntry.ContainsKey(ldapAttribute.Name))
                        dictionaryEntry.Add(ldapAttribute.Name, prop.GetValue(entry));
                }
            }

            foreach (var attr in attributes)
            {
                var attrName = !string.IsNullOrEmpty(attr.CustomAttributeName) ? attr.CustomAttributeName : attr.Attribute.ToString();

                if (attr.DataType == AttributeType.String)
                {
                    if (String.IsNullOrEmpty(attr.Value))
                        dictionaryEntry.Add(attrName, null);
                    else
                        dictionaryEntry.Add(attrName, attr.Value);
                }
                else if (attr.DataType == AttributeType.Int)
                {
                    if (String.IsNullOrEmpty(attr.Value))
                        dictionaryEntry.Add(attrName, 0);
                    else
                        dictionaryEntry.Add(attrName, Convert.ToInt32(attr.Value));
                }

                else if (attr.DataType == AttributeType.Boolean)
                {
                    if (String.IsNullOrEmpty(attr.Value))
                        dictionaryEntry.Add(attrName, null);
                    else
                        dictionaryEntry.Add(attrName, Boolean.Parse(attr.Value));
                }
                else
                    throw new ArgumentException("Non supported type for property " + attr.CustomAttributeName + ".");
            }
            return dictionaryEntry;
        }


        // SetDirectoryEntryAttributesiin virheilmoitus, jos attribuutin arvon muutos ei onnistu
        public DirectoryEntry SetDirectoryEntryAttributes(DirectoryEntry entry, Dictionary<string, object> attributes, bool isUpdate)
        {
            foreach (var attribute in attributes)
            {
                try
                {
                    if (attribute.Value != null)
                    {
                        entry.Properties[attribute.Key].Value = attribute.Value;
                    }
                    else
                    {
                        // attribuutin arvo oli tyhjä, seuraavaksi tyhjennös
                        if (isUpdate == true)
                        { // tyhjennä arvo 
                            entry.Properties[attribute.Key].Clear();
                        }
                    }
                }
                catch (Exception e)
                { throw new Exception("DirectoryEntry.Properties failed with key '" + attribute.Key + "'. Check that key is in schema and/or it was right. (it could be case sensitive). Exception: " + e.ToString()); }
            }
            return entry;
        }

        public void Dispose()
        {
            _rootEntry.Dispose();
            _rootEntry = null;
        }
    }
}
