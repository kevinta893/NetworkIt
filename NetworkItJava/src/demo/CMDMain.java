package demo;

import networkit.Client;
import networkit.Client.EventListener;
import networkit.Message;


/**
 * Command line demo for NetworkIt Java
 * @author kta
 */
public class CMDMain {

	public static void main(String[] args)
	{
		//create connection and add event listeners
		Client client = new Client("demo_test_username", "http://localhost", 8000);
		client.addConnectListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				//TODO your code here
				System.out.println("Client Connected");
			}
		});
		
		client.addMessageListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				//TODO your code here
				Message object = (Message) args[0];
				System.out.println("Message Recieved: " + object.toString());
				
			}
		});
		
		client.addDisconnectListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				//TODO your code here
				System.out.println("Client Disconnected");
			}
		});
		
		client.addErrorListener(new EventListener() {
			@Override
			public void call(Object sender, Object... args) {
				//TODO your code here
				Exception exception = (Exception) args[0];
				System.out.println("Error!");
				exception.printStackTrace();
			}
		});
		client.startConnection();
		
		
		
		//TODO Your code here!
		Message m = new Message("Poke");
		m.addField("num1", Integer.toString(66));
		m.addField("num2", Integer.toString(888));
		m.deliverToSelf = true;						//give ourselves a copy
		client.sendMessage(m);
	}
}
