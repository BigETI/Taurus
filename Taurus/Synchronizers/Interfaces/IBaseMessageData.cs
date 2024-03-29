﻿using Taurus.Validators;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents base message data
    /// </summary>
    public interface IBaseMessageData : IValidable
    {
        /// <summary>
        /// Message type
        /// </summary>
        string? MessageType { get; }
    }
}
