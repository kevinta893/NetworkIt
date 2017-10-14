# NetworkIt Arduino
An arduino port of the NetworkIt Library. Uses Node.js, serialport, and socket.io-client to connect the Arduino over the serial port and give it access to the internet.

## How to use
To use this library, you need to have installed Node.js (v7.5.0 is recommended). First configure the desired username, url, and port number for the connection in **Node SerialPort/arduino_network.js**:
```javascript
var username = 'demo_test_username';
var url = 'http://localhost';
var port = 8000;
```

Now install the required node libraries (or use extract node_modules.zip in the same folder)
```bash
npm install socket.io@2.0.3
npm install serialport@5.0.0
```

Then, everytime you need to use networking you must start the node server:
```bash
node arduino_network.js
```

You will have to terminate this client server every time you upload code to your arduino or you will get a "Port in use" error from the Arduino IDE. It is recommended that you use the Serial Monitor until you need to use the networking functionality. Remember to use the appropriate **Baud Rate**.

Refer to **Firmware/networkit_template** for the template firmware. Simply refer to the marked **TODOs** and insert your code as you please. The template also has sample code to show you how to work with the Message class and events.

### Running the demo
A wiring diagram has been provided for you. Then upload the Template firmware and start the node connector program to start using the demo. When pushing the push button, it should send a network message which gets delivered back to the Arduino blinking the light (it blinks on any demo message from the other platforms)

## Limitations (troubleshooting)
The Arduino has a limited SDRAM space (the Uno Rev3 having only 2k RAM total). Thus there have been memory optimizations from ArduinoJSON and the NetworkIt Library itself. Some points of the code if memory optimization is a must:
* See variables *jsonWriteBuffer* and *jsonReadBuffer*, these can affect how much data can be read from a single string message. You may want to trim your message keys and values down if you find that messages are not reading.
* See constant *MAX_FIELDS* in libraries/Message.h, this is the limit of how many fields that can exist in a message at one time. Change as necessary.

## Libraries Used:
* socket.io-client@2.0.3
* serialport@5.0.0
* [ArduinoJson-v5.11.1](https://github.com/bblanchon/ArduinoJson)
