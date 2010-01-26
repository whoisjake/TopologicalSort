using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DevelopStuff.Examples.TopologicalSort.Library
{
    /// <summary>
    /// Used to declare a one-to-many relationship with another model type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ParentAttribute : Attribute
    {
        #region Fields

        private string _relatedPropertyName;
        private Type _relatedModelType;

        #endregion

        #region Constructors

        /// <summary>
        /// Defines the tagged property as a parent of the current type.
        /// </summary>
        /// <param name="relatedModelType">The related entity type.</param>
        public ParentAttribute(Type relatedModelType)
        {

        }

        /// <summary>
        /// Defines the tagged property as a parent of the current type.
        /// </summary>
        /// <param name="relatedModelType">The related entity type.</param>
        /// <param name="relatedPropertyName">The related property (used for synchronization purposes).</param>
        public ParentAttribute(Type relatedModelType, string relatedPropertyName)
        {
            this._relatedModelType = relatedModelType;
            this._relatedPropertyName = relatedPropertyName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the associated type.
        /// </summary>
        public Type RelatedModelType
        {
            get { return this._relatedModelType; }
        }

        /// <summary>
        /// Gets the name of the related property.
        /// </summary>
        public string RelatedPropertyName
        {
            get { return _relatedPropertyName; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a list of AssociatedTo attributes for the specified type.
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static Dictionary<string, ParentAttribute> ForClass(Type modelType)
        {
            Dictionary<string, ParentAttribute> list = new Dictionary<string, ParentAttribute>();

            foreach (PropertyInfo prop in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object[] atts = prop.GetCustomAttributes(typeof(ParentAttribute), true);

                if (atts.Length > 0)
                    list.Add(prop.Name, (ParentAttribute)atts[0]);
            }

            return list;
        }

        /// <summary>
        /// Gets the <see cref="RelationshipAttribute"/> attached to a specified property.
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static ParentAttribute ForProperty(Type modelType, string propertyName)
        {
            if (modelType == null)
                throw new ArgumentNullException("modelType");

            PropertyInfo prop = modelType.GetProperty(propertyName);

            if (prop == null) throw new ArgumentException(string.Format("The property {0} does not exist on type {1}", propertyName, modelType));

            object[] atts = prop.GetCustomAttributes(typeof(ParentAttribute), true);

            if (atts.Length > 0) return atts[0] as ParentAttribute;
            else return null;
        }

        #endregion
    }
}
