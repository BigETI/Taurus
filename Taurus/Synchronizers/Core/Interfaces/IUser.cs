﻿using System;
using Taurus.Connectors;

namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents an user
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// User GUID
        /// </summary>
        Guid GUID { get; }

        /// <summary>
        /// Peer
        /// </summary>
        IPeer Peer { get; }
    }
}