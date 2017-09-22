
#include "libraries/ArduinoJson.h"
#include "libraries/Message.h"


int ledPin = 2;


void setup() 
{
  networkit_setup();      //please leave here

  //Your code here
  pinMode(ledPin, OUTPUT);

}


void loop() 
{
  networkit_loop();       //please leave here

  //Your code here
  
}


void messageEvent(Message& message)
{
  //your code here


  //demo, blinks some number of times depending on the message sent
  if (message.subject->equals("Poke!"))
  {
    int num1 = -1;
    num1 = message.getField("count")->toInt();
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
  //your code here
  
}

void disconnectEvent()
{
  //your code here
}

void errorEvent(JsonObject& message)
{
  //your code here
}




//=====================================================================================
//networkit library
//Arduino Uno 2K RAM
//Arduino Mega 2560 8K Ram


String inString;


void networkit_setup()
{
  Serial.begin(115200);
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




//if your message isnt being read properly, increase this buffer, see calculator: https://bblanchon.github.io/ArduinoJson/assistant/
StaticJsonBuffer<600> jsonBuffer;

void parseCommand(String cmd)
{
  //Serial.println("Recv=" + inString);
  JsonObject& eventJson = jsonBuffer.parseObject(cmd);

  const char* eventName = eventJson["event_name"];
  JsonObject& args = eventJson["args"];

  emitEvent(eventName, args);

  //messageRecieved(args);

  jsonBuffer.clear();     //clear buffer so we're ready for the next command
}

void emitEvent(const char* eventName, JsonObject& args)
{
  if (strcmp(eventName, "message") == 0)
  {
    //message got

    //convert to message object
    String subject = args["subject"];
    Message& m = *new Message(subject);

    JsonArray& fields = args["fields"];
    for (int i =0 ; i < fields.size() ; i++)
    {

      JsonObject& fieldObj = fields[i];
      String key = fieldObj["key"];
      String value = fieldObj["value"];
      m.addField(key, value);
    }
    
    messageEvent(m);
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

