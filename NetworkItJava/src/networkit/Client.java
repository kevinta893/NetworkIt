package networkit;

import java.net.URISyntaxException;
import io.socket.client.IO;
import io.socket.client.Socket;

public class Client {

	public Client(){
		
	}
	
	public Client(String username, String url, int port){
		try {
			 Socket socket = IO.socket(url + ":" + port);
			 socket.connect();
		} catch (URISyntaxException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}
