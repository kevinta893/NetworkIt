package networkit;

import java.net.URISyntaxException;

import org.json.JSONObject;

import io.socket.client.IO;
import io.socket.client.Socket;
import io.socket.emitter.Emitter.Listener;

public class Client {

	
	private String username = "demo_test_username";
	private String url = "http://581.cpsc.ucalgary.ca";
	private int port = 8000;
	
	private Socket client;
	
	public Client()
	{
		
	}
	
	public Client(String username, String url, int port)
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
					client.emit("client_connect", connectMsg);
					System.out.println("Connection successful.");
				}
			 });
			 
			 this.client.on(Socket.EVENT_MESSAGE, new Listener() {
				
				@Override
				public void call(Object... args) {
					System.out.println((args[0].toString()));
				}
			 });
			 
			 this.client.on(Socket.EVENT_DISCONNECT, new Listener() {
					
				@Override
				public void call(Object... args) {
					System.out.println("Client disconnected.");
				}
			 });
			 
			 this.client.on(Socket.EVENT_ERROR, new Listener() {
					
				@Override
				public void call(Object... args) {
					System.out.println("Error occured!");
				}
			 });
			 
			 this.client.connect();
		} catch (URISyntaxException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}
