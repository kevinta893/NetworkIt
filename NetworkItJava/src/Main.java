import org.json.JSONObject;

import networkit.Client;
import networkit.Client.EventListener;
import networkit.Message;

public class Main {

	public static void main(String[] args)
	{
		//create connection and add event listeners
		Client client = new Client("demo_test_username", "http://localhost", 8000);
		client.addConnectListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				System.out.println("Client Connected");
			}
		});
		
		client.addMessageListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				Message object = (Message) args[0];
				System.out.println("Message Recieved: " + object.toString());
				
			}
		});
		
		client.addDisconnectListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				System.out.println("Client Disconnected");

			}
		});
		
		client.addErrorListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				Exception exception = (Exception) args[0];
				System.out.println("Error!");
				exception.printStackTrace();
			}
		});
		client.startConnection();
		
		
		
		//Your code here!
		Message m = new Message("Poke");
		m.addField("num1", Integer.toString(66));
		m.addField("num2", Integer.toString(888));
		m.deliverToSelf = true;						//give ourselves a copy
		client.sendMessage(m);
	}
}
