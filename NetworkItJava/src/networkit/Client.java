package networkit;

import java.net.URISyntaxException;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;

import org.json.JSONArray;
import org.json.JSONObject;

import io.socket.client.IO;
import io.socket.client.Socket;
import io.socket.emitter.Emitter.Listener;

public class Client {

	
	private String username;
	private String url;
	private int port;
	
	private Socket client;
	
	private Map<String, LinkedList<EventListener>> eventListeners = new HashMap<String, LinkedList<EventListener>>();

	
	public Client()
	{
		//default settings
		this("demo_test_username", "http://581.cpsc.ucalgary.ca", 8000);
	}
	
	public Client(String username, String url, int port)
	{
		if (url.indexOf("http://") != 0)
             throw new IllegalArgumentException("URL must start with http://");

		this.username = username;
		this.url = url;
		this.port = port;
		
		eventListeners.put("connect", new LinkedList<EventListener>());
		eventListeners.put("message", new LinkedList<EventListener>());
		eventListeners.put("disconnect", new LinkedList<EventListener>());
		eventListeners.put("error", new LinkedList<EventListener>());
		
	}
	
	public void startConnection()
	{

		try 
		{
			
			 this.client = IO.socket(url + ":" + port);
			 
			 this.client.on(Socket.EVENT_CONNECT, new Listener() 
			 {
				@Override
				public void call(Object... args) 
				{
					JSONObject connectMsg = new JSONObject();
					connectMsg.put("username", username);
					connectMsg.put("platform", "Java");
					
					client.emit("client_connect", connectMsg);
					
					emitEvent("connect", args);
				}
			 });
			 
			 this.client.on(Socket.EVENT_MESSAGE, new Listener() 
			 {
				
				@Override
				public void call(Object... args) {			
					//Emits a single JSONObject, convert to Message
					JSONObject jsonObj = (JSONObject) args[0];
					
					//convert to message
					Message m = new Message(jsonObj.getString("subject"));
					m.deliverToSelf = jsonObj.getBoolean("deliverToSelf");
					
					JSONArray jsonFields = (JSONArray) jsonObj.get("fields");
					for (Object o : jsonFields)
					{
						JSONObject fieldObj = (JSONObject) o;
						m.addField(fieldObj.getString("key"), fieldObj.getString("value"));
					}
					
					emitEvent("message", m);
				}
			 });
			 
			 this.client.on(Socket.EVENT_DISCONNECT, new Listener() 
			 {
					
				@Override
				public void call(Object... args) {
					emitEvent("disconnect", args);
				}
			 });
			 
			 this.client.on(Socket.EVENT_ERROR, new Listener() 
			 {
				 
				@Override
				public void call(Object... args) {
					emitEvent("error", args);
				}
			 });
			 
			 this.client.connect();
		} catch (URISyntaxException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void closeConnection()
	{
		this.client.close();
	}
	
	
	
	public void sendMessage(Message m)
	{
		
		//encapsulate message with username header
		JSONObject sendObj = new JSONObject();
		sendObj.put("username", this.username);
		sendObj.put("subject", m.subject);
		sendObj.put("deliverToSelf", m.deliverToSelf);
		
		JSONArray sendArr = new JSONArray();
		for (Field f : m.getFields()){
			JSONObject fieldJson = new JSONObject();
			fieldJson.put("key", f.key);
			fieldJson.put("value", f.value);
			sendArr.put(fieldJson);
		}
		sendObj.put("fields", sendArr);
		
		this.client.emit("message", sendObj);
	}
	
	
	//===============
	//events
	
	public void emitEvent(String eventName, Object... args)
	{
		LinkedList<EventListener> listeners = eventListeners.get(eventName);
		
		for (EventListener eListener : listeners)
		{
			eListener.call(this, args);
		}
	}
	
	
	public void addConnectListener(EventListener l)
	{
		eventListeners.get("connect").add(l);
	}
	
	public void addDisconnectListener(EventListener l)
	{
		eventListeners.get("disconnect").add(l);
	}
	
	public void addMessageListener(EventListener l)
	{
		eventListeners.get("message").add(l);
	}
	
	public void addErrorListener(EventListener l)
	{
		eventListeners.get("error").add(l);
	}
	
	
	public interface EventListener{
		public void call(Object sender, Object... args);
	}
}
