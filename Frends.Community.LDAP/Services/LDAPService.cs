﻿using Frends.Community.LDAP.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

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

        private DirectoryEntry CreateEntry(string name, string organizationUnit, string schemaClass, Dictionary<string, object> entry)
        {
            var parent = _rootEntry;
            if (!string.IsNullOrEmpty(organizationUnit))
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
                    return res?.GetDirectoryEntry();
                }
            }
            catch (Exception ex)
            {
                string message = "Failed finding directory entry with path '{0}' under path '{1}'." + ex.Message;
                string rootPath = searchRoot != null ? searchRoot.Path : "";
                throw new ArgumentException(string.Format(message, path, rootPath), ex);
            }
        }

        private bool SaveEntry(DirectoryEntry entry, bool saveRootEntry = true)
        {
            try
            {
                if (saveRootEntry)
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

        public DirectoryEntry CreateAdUser(CreateADuser user)
        {
            var entryAttributes = GetEntryAttributes(user, user.OtherAttributes);
            if (!user.CN.ToUpper().StartsWith("CN="))
                user.CN = "CN=" + user.CN;

            var entry = CreateEntry(user.CN, user.Path, LdapClasses.User, entryAttributes);
            SaveEntry(entry);

            foreach (var flag in user.ADFlags)
            {
                entry.SetAccountFlag(flag.FlagType, flag.Value);
            }

            SaveEntry(entry);
            return entry;
        }

        public DirectoryEntry DeleteAdUser(DirectoryEntry entry)
        {
            entry.DeleteTree();
            entry.CommitChanges();
            return entry;
        }

        public DirectoryEntry RenameAdUser(DirectoryEntry entry, string newName)
        {
            entry.Rename("CN=" + newName);
            entry.CommitChanges();
            return entry;
        }

        public DirectoryEntry UpdateAdUser(UpdateADuser user)
        {
            //if (!user.CN.ToUpper().StartsWith("CN="))
            //    user.CN = "CN=" + user.CN;

            var entry = FindPath(_rootEntry, user.DN);
            var entryAttributes = GetEntryAttributes(user, user.OtherAttributes);
            SetDirectoryEntryAttributes(entry, entryAttributes, true);

            foreach (var flag in user.ADFlags)
            {
                entry.SetAccountFlag(flag.FlagType, flag.Value);
            }

            SaveEntry(entry);
            return entry;
        }

        /// <summary>
        /// Searches for the collection of the objects based on given filter. 
        /// </summary>
        /// <param name="filter">The attribute to filter the search by</param>
        /// <returns> The list of the DirectoryEntry(s) objects</returns>
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
                    using (SearchResultCollection resultCollection = s.FindAll())
                    {
                        foreach (SearchResult item in resultCollection)
                        {
                            ret.Add(item.GetDirectoryEntry());
                        }
                        return ret;
                    
                    }
                }
            }
            catch (Exception ex)
            {
                var message = "Failed finding objects with filter {0}:'." + ex.Message;
                throw new ArgumentException(string.Format(message, filter), ex);
            }
        }

        /// <summary>
        /// Searches for the collection of the objects based on given filter. 
        /// PropertiesToLoad specify which properties AD returns. Empty array returns object's all properties.
        /// Also this method returns current result set. If you need all properties AND live version, use AD_FetchObjects() task.
        /// </summary>
        /// <param name="filter">The attribute to filter the search by</param>
        /// <param name="propertiesToLoad">Array of properties to load. Empty array loads all properties.</param>
        /// <param name="pageSize">DirectorySearches paging on/off. 0 = off</param>
        /// <returns> The list of the DirectoryEntry(s) objects</returns>
        public List<SearchResult> SearchObjectsByFilterSpecifyProperties(string filter, string[] propertiesToLoad, int pageSize)
        {
            var ret = new List<SearchResult>();
            try
            {
                using (DirectorySearcher ds = new DirectorySearcher(_rootEntry))
                {
                    ds.SearchScope = SearchScope.Subtree;
                    ds.ReferralChasing = ReferralChasingOption.All;
                    ds.Filter = filter;

                    // Paging on/off
                    if (pageSize > 0)
                    {
                        ds.PageSize = pageSize;
                    }

                    // Specify properties to load -> better performance
                    foreach (var prop in propertiesToLoad)
                    {
                        ds.PropertiesToLoad.Add(prop);
                    }

                    using (var resultCollection = ds.FindAll())
                    {
                        foreach (SearchResult item in resultCollection)
                        {
                            ret.Add(item);
                        }
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                string message = "Failed searching objects with filter {0}:'." + ex.Message;
                throw new ArgumentException(string.Format(message, filter), ex);
            }
        }


        #region Windows Users
        public DirectoryEntry CreateWindowsUser(WindowsUser user)
        {
            var entryAttributes = GetEntryAttributes(user, user.OtherAttributes);
            var entry = CreateEntry(user.UserName, null, LdapClasses.User, entryAttributes);
            entry.Invoke("SetPassword", user.Password);
            entry.Invoke("Put", "Description", user.Description );
            SaveEntry(entry, false);
            return entry;
        }

        public DirectoryEntry UpdateWindowsUser(WindowsUser user)
        {
            var entry = SearchEntry(user.UserName, LdapClasses.User);
            entry.Invoke("SetPassword", user.Password);
            entry.Invoke("Put", "Description", user.Description);
            SaveEntry(entry, false);
            return entry;
        }

        public bool AddAdUserToGroup(string dn, IEnumerable<string> groups)
        {
            var entry = FindPath(_rootEntry, dn);
            foreach (var groupName in groups)
            {
                var groupEntry = SearchEntry(groupName, LdapClasses.Group);
                if (!IsMemberOf(entry, groupEntry))
                {
                    groupEntry.Invoke(LdapMethods.Add,  entry.Path);
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
                    groupEntry.Invoke(LdapMethods.Add, userEntry.Path);
                }
            }

            SaveEntry(userEntry, false);
            return true;
        }
        #endregion


        private static bool IsMemberOf(DirectoryEntry userEntry, DirectoryEntry groupEntry)
        {
            return (bool)groupEntry.Invoke(LdapMethods.IsMember,  userEntry.Path );
        }

        public DirectoryEntry SearchEntry(string name, string schemaClass)
        {
            return _rootEntry.Children.Find(name, schemaClass);
        }



        private Dictionary<string, object> GetEntryAttributes(object entry, IEnumerable<EntryAttribute> attributes)
        {
            var dictionaryEntry = new Dictionary<string, object>();

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

                if (string.IsNullOrWhiteSpace(attr.Value))
                {
                    if (attr.DataType == AttributeType.Int)
                        dictionaryEntry.Add(attrName, 0);
                    else
                        dictionaryEntry.Add(attrName, null);
                }
                else
                {
                    switch (attr.DataType)
                    {
                        case AttributeType.String:
                            dictionaryEntry.Add(attrName, attr.Value);
                            break;
                        case AttributeType.Int:
                            dictionaryEntry.Add(attrName, Convert.ToInt32(attr.Value));
                            break;
                        case AttributeType.Boolean:
                            dictionaryEntry.Add(attrName, bool.Parse(attr.Value));
                            break;
                        case AttributeType.JSONArray:
                            dictionaryEntry.Add(attrName, Newtonsoft.Json.Linq.JToken.Parse(attr.Value).ToArray());
                            break;
                        default:
                            throw new ArgumentException("Non supported type for property " + attr.CustomAttributeName + ".");
                    }
                }
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
                    if (attribute.Value != null && !string.IsNullOrEmpty(attribute.Value.ToString()))
                    {
                        entry.Properties[attribute.Key].Value = attribute.Value;
                    }
                    else
                    {
                        // attribuutin arvo oli tyhjä, seuraavaksi tyhjennös
                        if (isUpdate)
                        { // tyhjennä arvo 
                            entry.Properties[attribute.Key].Clear();
                        }
                    }
                }
                catch (Exception e)
                { throw new Exception("DirectoryEntry.Properties failed with key '" + attribute.Key + "'. Check that key is in schema and/or it was right. (it could be case sensitive). Exception: " + e); }
            }
            return entry;
        }

        public void Dispose()
        {
            _rootEntry.Dispose();
            _rootEntry = null;
        }

        public bool RemoveFromGroups(string targetDn, IEnumerable<string> groups)
        {
            DirectoryEntry targetEntry = FindPath(_rootEntry, targetDn);
            foreach (string groupName in groups)
            {
                DirectoryEntry groupEntry;
                try
                {
                    groupEntry = SearchEntry(groupName, LdapClasses.Group);
                }
                catch (Exception e)
                {
                    throw new Exception($"Exception occured while trying to find group {groupName} from {_rootEntry.Path}", e);
                }

                if (groupEntry == null)
                {
                    throw new Exception($"{groupName} was not found from {_rootEntry.Path}");
                }

                try
                {
                    if (IsMemberOf(targetEntry, groupEntry))
                    {
                        groupEntry.Invoke("remove", targetEntry.Path);
                        SaveEntry(groupEntry, false);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Something went wrong while removing user {targetDn} from group {groupName}", e);
                }
            }
            return SaveEntry(targetEntry, false);
        }

        /// <summary>
        /// Move a object to another OU. Returns: LdapResult class, which carries a copy of the updated object.
        /// </summary>
        public DirectoryEntry MoveAdObject(MoveObject adObject)
        {
            if (!adObject.CN.ToUpper().StartsWith("CN="))
                adObject.CN = "CN=" + adObject.CN;

            using (DirectoryEntry theObjectToMove = FindPath(_rootEntry, adObject.CN + "," + adObject.Path))
                using (DirectoryEntry theNewParent = FindPath(_rootEntry, adObject.NewPath))
                {
                    theObjectToMove.MoveTo(theNewParent);
                    return FindPath(_rootEntry, adObject.CN + "," + adObject.NewPath);
                }
        }
    }
}
