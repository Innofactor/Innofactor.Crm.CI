namespace Cinteros.Crm.Utils.Shuffle
{
    /// <summary>Property container class for Shuffle Counters</summary>
    /// <remarks>To be used by consuming application to display shuffle progress.
    /// Counters with value -1 shall be considered as "not updated".</remarks>
    public class ShuffleCounter
    {
        #region Public Properties

        /// <summary>Name of currently shuffled block.</summary>
        public string Block { get; set; } = null;

        /// <summary>Block number within current module currently being shuffled.</summary>
        public int BlockNo { get; set; } = -1;

        /// <summary>Total number of blocks to shuffle in current module.</summary>
        public int Blocks { get; set; } = -1;

        /// <summary>Name of currently shuffled record.</summary>
        public string Item { get; set; } = null;

        /// <summary>Record number within current block currently being shuffled.</summary>
        /// <remarks>For solution import this will indicate percent complete.</remarks>
        public int ItemNo { get; set; } = -1;

        /// <summary>Total number of records to shuffle in current block.</summary>
        public int Items { get; set; } = -1;

        /// <summary>Name of currently shuffled module.</summary>
        public string Module { get; set; } = null;

        /// <summary>Module that is currently shuffled.</summary>
        public int ModuleNo { get; set; } = -1;

        /// <summary>Total number of modules to shuffle.</summary>
        public int Modules { get; set; } = -1;

        #endregion Public Properties
    }
}