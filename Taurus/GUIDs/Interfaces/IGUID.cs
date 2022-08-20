﻿using System;
using Taurus.Validators;

/// <summary>
/// Taurus GUIDs namespace
/// </summary>
namespace Taurus.GUIDs
{
    /// <summary>
    /// An interface that represents a GUID
    /// </summary>
    /// <typeparam name="TGUID">GUID type</typeparam>
    public interface IGUID<TGUID> : IValidable, IComparable<TGUID>, IEquatable<TGUID> where TGUID : IGUID<TGUID>
    {
        /// <summary>
        /// GUID
        /// </summary>
        Guid GUID { get; }
    }
}