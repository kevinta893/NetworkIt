
#include "libraries/ArduinoJson.h"
#include "libraries/Message.h"


int ledPin = 2;
int buttonPin = 5;
bool pressed = false;
int messageCount = 0;

void setup() 
{
  networkit_setup();      //please leave here, Serial.begin(115200) has been called for you

  //TODO Your code here
  pinMode(ledPin, OUTPUT);
  pinMode(buttonPin, INPUT_PULLUP);
}



void loop() 
{
  networkit_loop();       //please leave here

  //TODO Your code here

  
  //send message demo
  if ((digitalRead(buttonPin) == LOW) && (pressed == false))
  {
    //pressed down
    pressed = true;
    Serial.println("Button pressed");

    Message* m = new Message("Poke!");
    m->deliverToSelf = true;
    m->addField("num1", new String(13));
    m->addField("num2", new String(5));
    messageCount++;
    m->addField("count", new String(messageCount));
    m->addField("message", "hello world");


    sendMessage(*m);
    delete m;
    
  } else if ((digitalRead(buttonPin) == HIGH) && (pressed == true)){
    //pressed up
    pressed = false;
  }
}


void messageEvent(Message& message)
{
  //TODO your code here


  //demo, blinks some number of times depending on the message sent
  if (message.subject->equals("Poke!"))
  {
    int num1 = -1;
    num1 = message.getField("count")->toInt();
    Serial.println("Got Count=" + num1);
    num1 = (num1 % 4) + 1;
    
    while(num1 > 0)
    {
      digitalWrite(ledPin, HIGH);
      delay(100);
      digitalWrite(ledPin, LOW);
      delay(100);
      num1--;
    }
    
  }
}


void connectEvent()
{
  //TODO your code here
}

void disconnectEvent()
{
  //TODO your code here
}

void errorEvent(JsonObject& message)
{
  //TODO your code here
}




//=====================================================================================
//networkit library
//Arduino Uno 2K RAM
//Arduino Mega 2560 8K Ram


String inString;


void networkit_setup()
{
  Serial.begin(115200);       //Uses Baud rate of 115200, set in serial monitor for debugging
}

void networkit_loop()
{
  while (Serial.available() > 0) 
  {
    int inChar = Serial.read();

    // if you get a newline, print the string,
    // then the string's value:
    if (inChar == '\n') {
      Serial.println("Recv=" + inString);

      parseCommand(inString);
       // clear the string for new input:
      inString = "";
    }

    inString += (char)inChar;
  }
}



//Sending
const char* NETWORK_SEND_HEADER = "send_networkit_message=";
const char* NETWORK_SEND_END_HEADER = "=send_end";
//if your message isnt being written properly, increase this buffer, see calculator: https://bblanchon.github.io/ArduinoJson/assistant/
StaticJsonBuffer<600> jsonWriteBuffer;
void sendMessage(Message& m)
{
  //create message object
  JsonObject& messJson = jsonWriteBuffer.createObject();
  messJson["subject"] = *m.subject;
  messJson["deliverToSelf"] = m.deliverToSelf;
  
  JsonArray& fieldsJson = messJson.createNestedArray("fields");
  for (int i = 0; i < m.getFieldCount() ;i++)
  {
    //add each field object
    JsonObject& f = jsonWriteBuffer.createObject();
    f["key"] = *m.getKey(i);
    f["value"] = *m.getValue(i);
    fieldsJson.add(f);
  }

  //send message to serial to send on socket.io
  Serial.print(NETWORK_SEND_HEADER);
  messJson.printTo(Serial);
  Serial.println(NETWORK_SEND_END_HEADER);
  
  jsonWriteBuffer.clear();
}


//Reading
//if your message isnt being read properly, increase this buffer, see calculator: https://bblanchon.github.io/ArduinoJson/assistant/
StaticJsonBuffer<600> jsonReadBuffer;
void parseCommand(String cmd)
{
  JsonObject& eventJson = jsonReadBuffer.parseObject(cmd);

  const char* eventName = eventJson["event_name"];
  JsonObject& args = eventJson["args"];
  emitEvent(eventName, args);

  jsonReadBuffer.clear();     //clear buffer so we're ready for the next command
}


void emitEvent(const char* eventName, JsonObject& args)
{
  if (strcmp(eventName, "message") == 0)
  {
    //message got

    //convert to message object
    String subject = args["subject"];
    Message* m = new Message(subject);

    JsonArray& fields = args["fields"];
    const char* key;
    const char* value;
    for (int i =0 ; i < fields.size() ; i++)
    {

      JsonObject& fieldObj = fields[i];
      key = fieldObj["key"];
      value = fieldObj["value"];
      m->addField(key, value);
    }
    
    messageEvent(*m);

    delete m;
  }
  else if (strcmp(eventName, "connect") == 0)
  {
    connectEvent();
  }
  else if (strcmp(eventName, "disconnect") == 0)
  {
    disconnectEvent();
  }
  else if (strcmp(eventName, "error") == 0)
  {
    errorEvent(args);
  }
}

