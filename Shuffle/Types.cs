namespace Cinteros.Crm.Utils.Shuffle.Types
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    internal enum ItemImportResult
    {
        None = 0,
        Created = 1,
        Updated = 2,
        Skipped = 3,
        Deleted = 4,
        Failed = 5
    }

    /// <summary>
    /// Defines how solution import is performed
    /// </summary>
    internal enum SolutionImportConditions
    {
        /// <summary>Solution did not exist and was imported.</summary>
        Create = 1,

        /// <summary>Solution existed and was now imported.</summary>
        Update = 2,

        /// <summary>Solution existed with same or higher version, and was not updated.</summary>
        Skip = 3
    }

    /// <summary>Dictionary containing Block name and corresponding CintDynEntityCollection</summary>
    [Serializable]
    public class ShuffleBlocks : Dictionary<string, EntityCollection>
    {   // Constructor added by suggestion from Code Analysis...
        #region Public Constructors

        public ShuffleBlocks()
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected ShuffleBlocks(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}