namespace NetworkIt
{
    public class Field
    {
        private string key;
        private string value;

        #region Instance Variables

        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
        }

        #endregion

        public Field(string name, string value)
        {
            this.key = name;
            this.value = value;
        }


        public override string ToString()
        {
            return this.key + " : " + this.value.ToString();
        }
    }
}
