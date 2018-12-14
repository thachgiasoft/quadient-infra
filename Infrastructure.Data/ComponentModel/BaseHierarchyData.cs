using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.ComponentModel
{
    

    // Summary:
    //     Exposes a node of a hierarchical data structure, including the node object
    //     and some properties that describe characteristics of the node. Objects that
    //     implement the System.Web.UI.IHierarchyData interface can be contained in
    //     System.Web.UI.IHierarchicalEnumerable collections, and are used by ASP.NET
    //     site navigation and data source controls.
    [DataContract]
    [Serializable]
    public class BaseHierarchyData
    {
        private string _key;
        private string _parentKey;


        public BaseHierarchyData(string key,string parentKey)
        {
            var any = this.GetType().GetProperty(key);
            var parentAny = this.GetType().GetProperty(parentKey);

            if (any == null || parentAny == null)
            {
                throw new MissingMemberException(string.Format("Missing Property {0} or {1}",key,parentKey));
            }
            _key = key;
            _parentKey = parentKey;
        }
        // Summary:
        //     Indicates whether the hierarchical data node that the System.Web.UI.IHierarchyData
        //     object represents has any child nodes.
        //
        // Returns:
        //     true if the current node has child nodes; otherwise, false.
        public bool HasChildren<T>(List<T> sourceList)
        {
            var id = this.GetType().GetProperty(_key).GetValue(this);
            var parentId = this.GetType().GetProperty(_parentKey).GetValue(this);

            if (id.Equals(parentId))
            {
                throw new InvalidOperationException(string.Format("Value of {0} property can't be equal to value of {1} property",_key,_parentKey));
            }
            return sourceList.Any(p => p.GetType().GetProperty(_parentKey).GetValue(p).ToString() == id);
        }

        //
        // Summary:
        //     Gets the hierarchical path of the node.
        //
        // Returns:
        //     A System.String that identifies the hierarchical path relative to the current
        //     node.
        public string Path { get { return string.Empty; } }
        
        /// <summary>
        /// Node' un hiyerarşideki konumunu belirtir.
        /// </summary>
        public int NodeLevel { get; set; }

        // Summary:
        //     Gets an enumeration object that represents all the child nodes of the current
        //     hierarchical node.
        //
        // Returns:
        //     An System.Web.UI.IHierarchicalEnumerable collection of child nodes of the
        //     current hierarchical node.
        public virtual List<T> GetChildren<T>(List<T> sourceList)
        {
            var valueOfKey = this.GetType().GetProperty(_key).GetValue(this);
            return sourceList.Where(p => p.Equals(this) == false && p.GetType().GetProperty(_parentKey).GetValue(p).Equals(valueOfKey)).ToList();
        }
        //
        // Summary:
        //     Gets an System.Web.UI.IHierarchyData object that represents the parent node
        //     of the current hierarchical node.
        //
        // Returns:
        //     An System.Web.UI.IHierarchyData object that represents the parent node of
        //     the current hierarchical node.
        public virtual T GetParent<T>(List<T> sourceList)
        {
            var valueOfParentKey = this.GetType().GetProperty(_parentKey).GetValue(this);
            return sourceList.Where(p => p.GetType().GetProperty(_key).GetValue(p) != null && p.GetType().GetProperty(_key).GetValue(p).Equals(valueOfParentKey)).FirstOrDefault();
        }
    }


}
