namespace NetworkIt
{
    public class Field
    {
        private string name;
        private string value;
     //   private string typeName;

        #region Instance Variables

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
        }

    /*   public string TypeName
        {
            get
            {
                return this.typeName;
            }
        }    */ 
      

        #endregion

       /*public Field(string name, string value, Type type)
       {
           this.name = name;
           this.fieldType = type;
           this.value = value;
           System.Diagnostics.Debug.WriteLine("TYPE IS " + this.fieldType);
       }*/

       public Field(string name, string value)
       {
           this.name = name;
          // this.typeName = typeName;
           this.value = value;
       //    System.Diagnostics.Debug.WriteLine("TYPE IS " + this.typeName);
       }


        public override string ToString()
        {
            return this.name + " :: " + this.value.ToString();
        }
    }
}
