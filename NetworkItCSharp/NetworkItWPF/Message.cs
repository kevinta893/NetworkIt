using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetworkIt
{
    public class Message
    {
        private string subject;
        private bool deliverToSelf = false;
        private List<Field> fields = new List<Field>();

        public string Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                this.subject = value;
            }
        }


        /// <summary>
        /// When true, allows the sender to also recieve the same message activating the same message event
        /// Default is false
        /// </summary>
        public bool DeliverToSelf
        {
            get
            {
                return this.deliverToSelf;
            }
            set
            {
                this.deliverToSelf = value;
            }
        }

        public List<Field> Fields
        {
            get
            {
                return this.fields;
            }
        }

        public Message(string subject)
        {
            this.subject = subject;
        }

        /// <summary>
        /// Adds a field to the message. Use this to automatically serialize your object to JSON and store it as a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"> Accepts all primitive types, and some classes. Classes are automatically converted into JSON</param>
        public void AddField<T>(string key, T value)
        {
            AddField(key, JsonConvert.SerializeObject(value));
        }

        public void AddField(string key, string value)
        {
            Field f = new Field(key, value);
            this.fields.Add(f);
        }

        /// <summary>
        /// Gets the value of a field given the key
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns default(T) if there is no value associated with the key</returns>
        public T GetField<T>(string key)
        {
            foreach (Field f in this.fields)
            {
                if (f.Key == key)
                {
                    return JsonConvert.DeserializeObject<T>(f.Value);
                }
            }

            return default(T);
        }

        public string GetField(string key)
        {
            return GetField<string>(key);
        }

        public override string ToString()
        {
            string ret = JsonConvert.SerializeObject(new
            {
                subject = this.subject,
                fields = this.fields
            });

            return ret;
        }
    }
}
