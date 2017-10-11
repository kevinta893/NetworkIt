package networkit;

import org.json.JSONException;
import org.json.JSONObject;

public class Field {

	public String key;
	public String value;
	
	public Field (String key, String value)
	{
		this.key = key;
		this.value = value;
	}
	
	public String toString(){
		return "{" + key + ":" + value + "}";
	}
	
	protected JSONObject toJSONObject()
	{
		JSONObject ret = new JSONObject();
		try{
			ret.put("put", key);
			ret.put("value", value);
		} catch (JSONException e) {
			e.printStackTrace();
		}
		return ret;
	}
}
