class Message{
  
  
  public:
    String* subject;
    bool deliverToSelf;
  
    Message(String& subject);
    ~Message();
    String* getField(String& key);
	String* getField(const char* key);
    void addField(String& key, String& value);
    String* toString();
    
  private:
    static const int MAX_FIELDS = 10;            //maximum number of fields per message. Increase if needed
    String _fieldsKeys[MAX_FIELDS];
    String _fieldsValues[MAX_FIELDS];
    int _fieldCount = 0;
    
};

Message::Message(String& subject)
{
  this->subject = &subject;
  this->deliverToSelf = false;
  //this->_fields;
  this->_fieldCount = 0;
}

Message::~Message()
{
  //delete this->_fields;
  delete this->subject;
}

String* Message::getField(String& key)
{
  for (int i = 0 ; i < _fieldCount ; i++)
  {
     if (key.equals(this->_fieldsKeys[i]) == 1 )
     {
         return &this->_fieldsValues[i];
     }
  }
  Serial.println("Field not found=" + key);
  return NULL;
}

String* Message::getField(const char* key)
{
  String keyCompare = key;
  return this->getField(keyCompare);
}

void Message::addField(String& key, String& value)
{
  if (_fieldCount >= MAX_FIELDS)
  {
    Serial.println("Error! MAX_FIELDS exceeded");
	return;
  }

  this->_fieldsKeys[_fieldCount] = key;
  this->_fieldsValues[_fieldCount] = value;
  this->_fieldCount++;

  //Serial.println(key + "," + value);
  //Serial.println(*f.key + "^&&^&^&," + *f.value);
	
}

String* Message::toString()
{
  return subject;
}