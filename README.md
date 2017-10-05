# NetworkIt
A static IP, socket.io based proxy server and client to allow different platforms to communciate to each other. Designed for CPSC 581 students at the University of Calgary, the kit allows students to quickly combine different plaforms and devices together for prototyping.

Originally developed by: David Ledo & Brennan Jones

## For Students
Download the appropriate releases for your project and import. There are instructions on how to import in each platform release. There is also a demo program in each platform that shows how to use each

Current supported platforms:
* Windows WPF 
* Windows 8 & 8.1 Phones (outdated)
* Unity (Windows, Mac, Linux)
* Python 2.7 (Raspberry Pi Compatiable)
* Arduino (with node.js v7.5.0)

## For Instructors
Request a static IP and server hosting from your IT department. 

**Installation**: 
Install socket.io (v1.7.3) with commands (or otherwise use the included node_modules): 
```bash
npm install socket.io@1.7.3
npm install dateformat@3.0.2
```
**Run with**
```bash
node server.js
```

## Developers
Refer to the WPF version which defines the protocol. Use the latest version to structure how the library should operate when porting to other platforms. Prioritize building quick connect and easy to use object serialization through JSON.
