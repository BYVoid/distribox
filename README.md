# Distribox

## What does Distribox do

Distribox is a file synchronization tool based on P2P algorithms. Clients can join a P2P network to share files on local file system automatically like Dropbox or Google Drive. By using Distribox, you can push your documents, photos or anything else in to a folder, and Distribox will synchronize them with the cloud. If you have multiple devices (laptop, phone, pad), Distribox enables you to synchronize files between them. They can also share folders with other users by specifying collaborators or making the folder public.

Distribox is born with sharing, because it copies all the files to all the other nodes in the P2P network. Imagine such a scene: a small group of people in a local area network that need to exchange files frequently. As long as one person adds, edits, or removes a file, all the other people can see the modification instantly on their local computers.

## Why is Distribox different

Unlike Dropbox or Google Drive which relies on a cloud storage service, the architecture of Distribox is totally decentralized. It means you do not have to set up a set of servers or a private cloud, while clients, or nodes, self-organize automatically. Users in a Distribox P2P network (we say a group) share all files in a folder, where one's modification reflects on all the others. If multiple users modify a file simultaneously, all versions will be perserved separately and every single user can see any version that exists and determine which to work on with.

We do not rely on a central server or a cloud because it is not secure to store our private data on untrusted third-party's hardwares. Any centralized approach of file sharing is vulnerable of failures of the central server. The central server could be controlled by the authority or system invaders, making our privacy in danger. On the other hand, maintaining a private cloud infrastructure costs lots of work and it is not reliable as expected.

In one word, Distribox is a P2P file sharing tool, which can be used as a file backup tool as well. While Dropbox and Google Drive are centralized systems owned by big companies which more concentrate on file synchronization with could to perserve the so-called 'safety', and sharing is an additional functionality.

Although Distribox is decentralized, you can set up a node on a central server, which can enhance reliability of the P2P system. We can call such a node moniter or daemon, meaning that the node is always online and does nothing but synchronizes everything.

## What are the objectives

Distribox is still under development. Our long-term objective is to build a full-featured open-source file sharing tool based on P2P. Two application scenarios are:

* Sharing documents within small groups
* Synchronizing personal files between multiple PCs

The first step is to implement the architecture of Distribox. The two main components are reliable P2P communication based on gossip protocol and file multiversion controlling. Functionalities we plan to implement first are listed below:

* Online nodes detection
* Version list propagation
* Peer-to-peer data transfer
* Local file system events monitering and new version creation
* Version tree management
* GUI for browsering versions and logs

## Download

https://sourceforge.net/projects/distribox/files/

## Source code

https://github.com/BYVoid/distribox

## Documentation for developers

http://distribox.net/
