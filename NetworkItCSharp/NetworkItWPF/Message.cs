using System;
using System.Collections.Generic;

namespace NetworkIt
{
    public class Message
    {
        private string name;
        private List<Field> fields = new List<Field>();

        public string Name
        {
            get
            {
                return this.name;
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
            this.name = messageName;
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

    }
}
