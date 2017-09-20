
#include "libraries/ArduinoJson.h"

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


void messageRecieved(JsonObject& message)
{

  const char* subject = message["subject"];
  Serial.println(subject);
  if (strcmp(subject, "Poke!") == 0)
  {
    digitalWrite(ledPin, HIGH);
    delay(500);
    digitalWrite(ledPin, LOW);
  }
}


//===============================================
//networkit library


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

  messageRecieved(args);

  jsonBuffer.clear();
}

void emitEvent(const char* eventName, JsonObject& args)
{
  
}

