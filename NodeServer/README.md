# Proxy Server
This is the heart of the API, the proxy server relays messages to other users on the network given a username. The username should be kept secret from others, but otherwise one can run their own instance with this server.

Use the provided bash script to run the server such that it restarts everytime it crashes.


## Installation

If you wish to run your see Hosting your own instance below. Basically run the NodeServer on your own machine and use localhost as the URL. This is useful for personal servers for when the instructor's server isn't running anymore.

If you wish to run a server for students, request a static IP and server hosting from your IT department. You may also wish to add a Domain Name so it is easier for students to access the server.

Installation: Install socket.io (v1.7.3) with commands (or otherwise use the included node_modules):
```bash
npm install socket.io@1.7.3
npm install dateformat@3.0.2
```

Run with

```bash
node server.js
```

