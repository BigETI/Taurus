using ENet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Taurus.Compressors;
using Taurus.Fragmenters;
using Taurus.Validators;

namespace Taurus.Connectors.ENet
{
    /// <summary>
    /// A class that describes an ENet connector
    /// </summary>
    public sealed class ENetConnector : AConnector, IENetConnector
    {
        /// <summary>
        /// ENet initialization mutex
        /// </summary>
        private static readonly Mutex eNetInitializationMutex = new Mutex();

        /// <summary>
        /// ENet initialization count
        /// </summary>
        private static uint eNetInitializationCount;

        /// <summary>
        /// Disconnection reasons
        /// </summary>
        private static readonly HashSet<EDisconnectionReason> disconnectionReasons =
            new HashSet<EDisconnectionReason>((EDisconnectionReason[])Enum.GetValues(typeof(EDisconnectionReason)));

        /// <summary>
        /// Connect to ENet addresses
        /// </summary>
        private readonly ConcurrentQueue<Address> connectToENetAddresses = new ConcurrentQueue<Address>();

        /// <summary>
        /// ENet peer ID to peer lookup
        /// </summary>
        private readonly Dictionary<uint, IPeer> peerIDToPeerLookup = new Dictionary<uint, IPeer>();

        /// <summary>
        /// Connector thread
        /// </summary>
        private readonly Thread connectorThread;

        /// <summary>
        /// Is connector thread running
        /// </summary>
        private volatile bool isConnectorThreadRunning = true;

        /// <summary>
        /// Listening to port
        /// </summary>
        private volatile ushort listeningToPort;

        /// <summary>
        /// Buffer
        /// </summary>
        private byte[] buffer = new byte[2048];

        /// <summary>
        /// ENet host service timeout time in milliseconds
        /// </summary>
        public uint ENetHostServiceTimeoutTime { get; }

        /// <summary>
        /// Maximal peer count
        /// </summary>
        public uint MaximalPeerCount { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">Host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt">Handles peer connection attempts</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        public ENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) : base(onHandlePeerConnectionAttempt, fragmenter, compressor)
        {
            if (eNetHostServiceTimeoutTime > Library.timeoutMaximum)
            {
                throw new ArgumentException
                (
                    $"ENet host service timeout time has to be set between 0 ms and {Library.timeoutMaximum} ms. Specified ENet host service timeout time: {eNetHostServiceTimeoutTime} ms",
                    nameof(eNetHostServiceTimeoutTime)
                );
            }
            if (maximalPeerCount > Library.maxPeers)
            {
                throw new ArgumentException
                (
                    $"Maximal peer count can not exceed {Library.maxPeers}; Specified maximal peer count: {maximalPeerCount}",
                    nameof(maximalPeerCount)
                );
            }
            ENetHostServiceTimeoutTime = eNetHostServiceTimeoutTime;
            MaximalPeerCount = maximalPeerCount;
            connectorThread = new Thread
            (
                () =>
                {
                    eNetInitializationMutex.WaitOne();
                    if (eNetInitializationCount == 0U)
                    {
                        Library.Initialize();
                    }
                    ++eNetInitializationCount;
                    eNetInitializationMutex.ReleaseMutex();
                    Host host = new Host();
                    try
                    {
                        ushort listened_to_port = 0;
                        while (isConnectorThreadRunning)
                        {
                            ushort listening_to_port = listeningToPort;
                            if (listened_to_port != listening_to_port)
                            {
                                listened_to_port = listening_to_port;
                                host.Dispose();
                                host = new Host();
                                if (listening_to_port == 0)
                                {
                                    host.Create(1, 0);
                                }
                                else
                                {
                                    host.Create
                                    (
                                        new Address
                                        {
                                            Port = listening_to_port
                                        },
                                        (int)maximalPeerCount,
                                        0
                                    );
                                }
                            }
                            else if (!host.IsSet)
                            {
                                host.Create(1, 0);
                            }
                            while (connectToENetAddresses.TryDequeue(out Address connect_to_e_net_address))
                            {
                                host.Connect(connect_to_e_net_address);
                            }
                            ProcessRequests();
                            while (host.Service((int)ENetHostServiceTimeoutTime, out Event network_event) > 0)
                            {
                                IPeer peer;
                                switch (network_event.Type)
                                {
                                    case EventType.Connect:
                                        if (!peerIDToPeerLookup.ContainsKey(network_event.Peer.ID))
                                        {
                                            peer = new ENetPeer(new PeerGUID(Guid.NewGuid()), this, network_event.Peer);
                                            peerIDToPeerLookup.Add(network_event.Peer.ID, peer);
                                            EnqueuePeerConnectionAttemptedEvent(peer);
                                        }
                                        break;
                                    case EventType.Disconnect:
                                        if (peerIDToPeerLookup.TryGetValue(network_event.Peer.ID, out peer))
                                        {
                                            EDisconnectionReason disconnection_reason = (EDisconnectionReason)network_event.Data;
                                            EnqueuePeerDisconnectedEvent
                                            (
                                                peer,
                                                disconnectionReasons.Contains(disconnection_reason) ?
                                                    disconnection_reason :
                                                    EDisconnectionReason.Invalid
                                            );
                                        }
                                        break;
                                    case EventType.Receive:
                                        if (peerIDToPeerLookup.TryGetValue(network_event.Peer.ID, out peer))
                                        {
                                            Packet packet = network_event.Packet;
                                            if (buffer.Length < packet.Length)
                                            {
                                                buffer =
                                                    new byte[packet.Length / buffer.Length * (((packet.Length % buffer.Length) == 0) ? 1 : 2) * buffer.Length];
                                            }
                                            packet.CopyTo(buffer);
                                            Marshal.Copy(packet.Data, buffer, 0, packet.Length);
                                            EnqueuePeerMessageReceivedEvent(peer, buffer.AsSpan(0, packet.Length));
                                            packet.Dispose();
                                        }
                                        break;
                                    case EventType.Timeout:
                                        if (peerIDToPeerLookup.TryGetValue(network_event.Peer.ID, out peer))
                                        {
                                            EnqueuePeerDisconnectedEvent(peer, EDisconnectionReason.TimedOut);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        host.Dispose();
                        peerIDToPeerLookup.Clear();
                        eNetInitializationMutex.WaitOne();
                        if (eNetInitializationCount > 0U)
                        {
                            --eNetInitializationCount;
                            if (eNetInitializationCount == 0U)
                            {
                                Library.Deinitialize();
                            }
                        }
                        eNetInitializationMutex.ReleaseMutex();
                    }
                }
            );
            connectorThread.Start();
        }

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <returns>New ENet connector</returns>
        public static ENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt
        ) => CreateNewENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, null, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt"></param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <returns>New ENet connector</returns>
        public static ENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter fragmenter
        ) => CreateNewENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, fragmenter, null);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static ENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            ICompressor compressor
        ) => CreateNewENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, null, compressor);

        /// <summary>
        /// Creates a new ENet connector
        /// </summary>
        /// <param name="eNetHostServiceTimeoutTime">ENet host service timeout time in milliseconds</param>
        /// <param name="maximalPeerCount">Maximal peer count</param>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New ENet connector</returns>
        public static ENetConnector CreateNewENetConnector
        (
            uint eNetHostServiceTimeoutTime,
            uint maximalPeerCount,
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) => new ENetConnector(eNetHostServiceTimeoutTime, maximalPeerCount, onHandlePeerConnectionAttempt, fragmenter, compressor);

        /// <summary>
        /// Tries to get ENet peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="eNetPeer">ENet peer</param>
        /// <returns></returns>
        private bool TryGettingENetPeer(IPeer peer, out IENetPeer? eNetPeer)
        {
            bool ret = false;
            if
            (
                (peer is IENetPeer e_net_peer) &&
                peerIDToPeerLookup.TryGetValue(e_net_peer.Peer.ID, out IPeer current_peer) &&
                (e_net_peer == current_peer)
            )
            {
                ret = true;
                eNetPeer = e_net_peer;
            }
            else
            {
                eNetPeer = null;
            }
            return ret;
        }

        /// <summary>
        /// Asserts that peer is an ENet peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="onPeerIsENetPeerAsserted">Gets invoked when a peer is an ENet peer asserted</param>
        private void AssertPeerIsENetPeer(IPeer peer, PeerIsENetPeerAssertedDelegate onPeerIsENetPeerAsserted)
        {
            if (TryGettingENetPeer(peer, out IENetPeer? e_net_peer))
            {
                onPeerIsENetPeerAsserted(e_net_peer!);
            }
        }

        /// <summary>
        /// Handles peer disconnection request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        protected override void HandlePeerDisconnectionRequest(IPeer peer, EDisconnectionReason disconnectionReason)
        {
            AssertPeerIsENetPeer
            (
                peer,
                (e_net_peer) =>
                {
                    peerIDToPeerLookup.Remove(e_net_peer.Peer.ID);
                    e_net_peer.Peer.Disconnect((uint)disconnectionReason);
                }
            );
        }

        /// <summary>
        /// Handles sending peer message request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        protected override void HandleSendingPeerMessageRequest(IPeer peer, ReadOnlySpan<byte> message)
        {
            if (TryGettingENetPeer(peer, out IENetPeer? e_net_peer))
            {
                Packet packet = default;
                packet.Create(message.ToArray(), PacketFlags.Reliable);
                e_net_peer!.Peer.Send(0, ref packet);
            }
        }

        /// <summary>
        /// Listens to the specified port
        /// </summary>
        /// <param name="port">Port</param>
        /// <exception cref="ArgumentException">When "port" is zero</exception>
        public void ListenToPort(ushort port)
        {
            if (port == 0)
            {
                throw new ArgumentException("Listening port can not be zero.", nameof(port));
            }
            listeningToPort = port;
        }

        /// <summary>
        /// Stops listening to port
        /// </summary>
        public void StopListening() => listeningToPort = 0;

        /// <summary>
        /// Connects to the specified network
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="port">Port</param>
        public void ConnectToENet(string host, ushort port)
        {
            StringValidator.ValidateStringIsNotEmptyOrHasNoWhitespaces(host, nameof(host));
            Validator.Validate(port, nameof(port), (input) => input > 0);
            Address address =
                new Address
                {
                    Port = port
                };
            address.SetHost(host);
            connectToENetAddresses.Enqueue(address);
        }

        /// <summary>
        /// Closes connection to all connected peers in this connector
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        public override void Close(EDisconnectionReason reason)
        {
            base.Close(reason);
            isConnectorThreadRunning = false;
            connectorThread.Join();
        }
    }
}
