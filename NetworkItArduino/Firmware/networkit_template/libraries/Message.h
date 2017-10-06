class Message{
  
  
  public:
    String* subject;
    bool deliverToSelf;
  
	Message(const char* subject);
    Message(String& subject);
    ~Message();
    String* getField(String* key);
	String* getField(const char* key);
	
    void addField(String* key, String* value);
	void addField(const char* key, const char* value);
	void addField(const char* key, String* value);
	void addField(String* key, const char* value);
	
    String* toString();
    int getFieldCount();
	String* getKey(int i);
	String* getValue(int i);
	
  private:
    static const int MAX_FIELDS = 5;            //maximum number of fields per message. Increase if needed
    String** _fieldsKeys;
    String** _fieldsValues;
    int _fieldCount = 0;
    
};

Message::Message(String& subject)
{
	this->subject = new String(subject);
	this->deliverToSelf = false;
	this->_fieldCount = 0;
	this->_fieldsKeys = new String*[MAX_FIELDS];
	this->_fieldsValues = new String*[MAX_FIELDS];
}

Message::Message(const char* subject)
{
	this->subject = new String(subject);
	this->deliverToSelf = false;
	this->_fieldCount = 0;
	this->_fieldsKeys = new String*[MAX_FIELDS];
	this->_fieldsValues = new String*[MAX_FIELDS];
}

Message::~Message()
{
	delete this->subject;
	//clear all items in arrays
	for (int i = 0 ; i < _fieldCount ; i++)
	{
		delete this->_fieldsKeys[i];
		delete this->_fieldsValues[i];
	}
	delete this->_fieldsKeys;
	delete this->_fieldsValues;
}

String* Message::getField(String* key)
{
	String** keyArray = this->_fieldsKeys;
	String** valueArray = this->_fieldsValues;
	
	for (int i = 0 ; i < _fieldCount ; i++)
	{
		String k = *keyArray[i];
		if (k.equals(*key) == 1 )
		{
			return valueArray[i];
		}
	}
	Serial.println("Field not found=" + *key);
	return NULL;
}

String* Message::getField(const char* key)
{
  String keyCompare = key;
  return this->getField(&keyCompare);
}

void Message::addField(String* key, String* value)
{
  if (_fieldCount >= MAX_FIELDS)
  {
    Serial.println("Error! MAX_FIELDS exceeded");
	return;
  }

  this->_fieldsKeys[_fieldCount] = key;
  this->_fieldsValues[_fieldCount] = value;
  this->_fieldCount++;
}

void Message::addField(const char* key, const char* value)
{
	String* keyString = new String(key);
	String* valueString = new String(value);
	addField(keyString, valueString);
}

void Message::addField(const char* key, String* value)
{
	String* keyString = new String(key);
	addField(keyString, value);
}

void Message::addField(String* key, const char* value)
{
	String* valueString = new String(value);
	addField(key, valueString);
}

String* Message::toString()
{
  return subject;
}

int Message::getFieldCount()
{
	return _fieldCount;
}

String* Message::getKey(int i)
{
	if (i >= this->_fieldCount)
	{
		return NULL;
	}
	return this->_fieldsKeys[i];
}

String* Message::getValue(int i)
{
	if (i >= this->_fieldCount)
	{
		return NULL;
	}
	return this->_fieldsValues[i];
}