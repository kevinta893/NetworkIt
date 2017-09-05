using System;
using System.Collections.Generic;

namespace NetworkIt
{
    public class Message
    {
        private string raw;
        private List<Field> fields = new List<Field>();

        public string Raw
        {
            get
            {
                return this.raw;
            }
        }

        public List<Field> Fields
        {
            get
            {
                return this.fields;
            }
        }

        public Message(string messageName)
        {
            this.raw = messageName;
        }

        public void AddField<T>(string name, T value)
        {
            Type type = value.GetType();
            //  Field f = new Field(name, value.ToString(), type.ToString());
            Field f = new Field(name, value.ToString());

            this.fields.Add(f);
        }

        public string GetField(string name)
        {
            foreach (Field f in this.fields)
            {
                if (f.Name == name)
                {
                    return f.Value;
                }
            }
            return null;
        }

        public override string ToString()
        {
            string ret = "{";
            foreach (Field f in fields)
            {
                ret += f.ToString() + ",";
            }

            return ret + "}";
        }
    }
}
