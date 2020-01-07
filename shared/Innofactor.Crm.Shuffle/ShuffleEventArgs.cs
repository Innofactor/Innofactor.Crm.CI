namespace Cinteros.Crm.Utils.Shuffle
{
    using System;

    /// <summary>Event args class passed in the RaiseShuffleEvent event</summary>
    public class ShuffleEventArgs : EventArgs
    {
        #region Private Fields

        private readonly ShuffleCounter counters;
        private readonly string msg;
        private readonly bool replaceLastMessage;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>Constructor</summary>
        /// <param name="s">Message</param>
        /// <param name="tb">Total number of blocks, -1 if not updated</param>
        /// <param name="cb">Current block, -1 if not updated</param>
        /// <param name="br">Total number of records in block, -1 if not updated</param>
        /// <param name="cr">Current record within block, -1 if not updated</param>
        /// <param name="repl">Bool indicating if this message should replace last message, for "fluent updates" of progress information</param>
        public ShuffleEventArgs(string s, int tb, int cb, int br, int cr, bool repl)
        {
            msg = s;
            counters = new ShuffleCounter() { Blocks = tb, BlockNo = cb, Items = br, ItemNo = cr };
            replaceLastMessage = repl;
        }

        /// <summary>Constructor for updated module, block or record names.</summary>
        /// <param name="m"></param>
        /// <param name="b"></param>
        /// <param name="r"></param>
        public ShuffleEventArgs(string m, string b, string r)
        {
            counters = new ShuffleCounter() { Block = b, Item = r };
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>Progress counters</summary>
        public ShuffleCounter Counters { get { return counters; } }

        /// <summary>Message from the Shuffle</summary>
        public string Message { get { return msg; } }

        /// <summary>Flag if this message should replace last message, for "fluent updates" of progress information</summary>
        public bool ReplaceLastMessage { get { return replaceLastMessage; } }

        #endregion Public Properties
    }
}