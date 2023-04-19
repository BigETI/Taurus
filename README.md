# Taurus

Taurus is a scalable multi-purpose and modular multiplayer library written in pure C♯ that is designed for to easily set up low-latency services such as for example online multiplayer games.

## Documentation

Once this library is at a stage where no major API changes may be applied, an extensive documentation with examples will be included in this repository or another website.

## Design philosophy

### Peer

Each connection is represented by a peer, which you can use to recieve from or send messages to.

### Connector

A connector is the simplest form of listening to or creating new connections to other hosts or instances.

A connector itself is enough to send or recieve raw messages, however it is not recommended for use cases, where your application needs to differentiate between different message types and may require facilities to perform synchronization and error handling.

### Synchronizer

Taurus provides classes to implement synchronizers, which handle message parsing and error handling.

A synchronizer is recommended to be used in conjunction with a connector.

It is possible to assign multiple connectors to a synchronizer in case your application needs to support multiple network communication protocols, adhere scaling or just different rules how messages should be compressed or fragmented.

### User

An user in this context is a peer that is acknowledged by a synchronizer.

Like any peer you can send messages to or recieve messages from an user, except in this case you have to deal with serializable objects instead of raw messages.

### Serializer

A serializer serializes raw messages to serilizable objects or deserializes serializable objects to raw messages.

### Compressor

A compressor allows to minimize the amount of data transmitted over the wire to improve performance.

### Fragmenter

A fragmenter fragments or defragments messages in such a way that every message is ensured to be unique.

## Origin

This library is a successor to the network stack implemented in [ElectrodZ Multiplayer](https://github.com/BigETI/ElectrodZMultiplayer).

## Real-life use cases

Currently Taurus is in use for an upcoming online multiplayer game made with the Unity Engine.

It is planned to use Taurus for future online multiplayer game projects.

## License

You can find the license of this project [here](https://github.com/BigETI/Taurus/blob/main/LICENSE): https://github.com/BigETI/Taurus/blob/main/LICENSE