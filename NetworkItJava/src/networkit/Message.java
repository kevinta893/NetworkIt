package networkit;

import java.util.LinkedList;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;


public class Message {

	public String subject = "";
	public boolean deliverToSelf = false;
	private List<Field> fields = new LinkedList<Field>();
	
	
	public Message(String subject)
	{
		this.subject = subject;
	}
	
	public void addField (String key, String value)
	{
		fields.add(new Field(key, value));
	}
	
	public String getField(String key)
	{
		for (Field sField : fields)
		{
			if (key.equals(sField.key))
			{
				return sField.value;
			}
		}
		
		return null;
	}
	
	public List<Field> getFields(){
		return fields;
	}
	
	public String toString()
	{
		JSONObject convert = new JSONObject();
		try{
			convert.put("subject", this.subject);
			convert.put("deliverToSelf", this.deliverToSelf);
			
			JSONArray jsonFields = new JSONArray();
			for (Field f : fields)
			{
				jsonFields.put(f.toJSONObject());
			}
			
			convert.put("fields", jsonFields);
			
			
		} catch (JSONException e) {
			e.printStackTrace();
		}
		return convert.toString();
	}
	
}
