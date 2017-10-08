package demo;

import java.awt.BorderLayout;
import java.awt.EventQueue;

import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.border.EmptyBorder;

import networkit.Client;
import networkit.Message;
import networkit.Client.EventListener;

import javax.print.attribute.standard.DateTimeAtCompleted;
import javax.swing.JButton;
import javax.swing.JTextField;
import java.awt.Color;
import java.awt.Component;

import javax.swing.JCheckBox;
import javax.swing.JFileChooser;
import javax.swing.JLabel;
import javax.swing.JOptionPane;

import java.awt.Font;
import javax.swing.JScrollPane;
import javax.swing.JTextPane;
import javax.swing.ScrollPaneConstants;
import javax.swing.SwingUtilities;

import java.awt.event.ActionListener;
import java.awt.print.Printable;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.PrintWriter;
import java.sql.Date;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.awt.event.ActionEvent;
import javax.swing.JTextArea;

/**
 * GUI Demo main
 * @author kta
 *
 */
public class DemoMain extends JFrame {

	private JPanel contentPane;
	private JTextField txtURL;
	private JTextField txtPort;
	private JTextField txtUsername;
	private JButton btnConnect;
	private JButton btnSend;
	private JPanel pnlStatus;
	private JTextPane lblLog;
	private JCheckBox chkDeliverToSelf;
	private Client client;
	
	private JFileChooser fc = new JFileChooser();

	
	private int messageCount = 0;
	
	
	private static final int DEFAULT_PORT = 8000;
	
	
	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				try {
					DemoMain frame = new DemoMain();
					frame.setVisible(true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	/**
	 * Create the frame.
	 */
	public DemoMain() {
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setBounds(100, 100, 600, 500);
		contentPane = new JPanel();
		contentPane.setBorder(new EmptyBorder(5, 5, 5, 5));
		setContentPane(contentPane);
		contentPane.setLayout(null);
		
		btnSend = new JButton("Send Poke!");
		btnSend.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent arg0) {
				Message m = new Message("Poke!");
	            m.deliverToSelf = chkDeliverToSelf.isSelected();
	            m.addField("num1", 3 + "");
	            m.addField("num2", 4 + "");
	            m.addField("count", "" + messageCount++);
	            client.sendMessage(m);
			}
		});
		btnSend.setFont(new Font("Tahoma", Font.PLAIN, 24));
		btnSend.setBounds(10, 11, 156, 63);
		contentPane.add(btnSend);
		
		JButton btnSaveLog = new JButton("Save Log...");
		btnSaveLog.addActionListener(new ActionListener() {
			
			public void actionPerformed(ActionEvent arg0) {
				//save the log
				
				//check if there's a log to even save
	            if (lblLog.getText().length() <= 0)
	            {
	            	JOptionPane.showMessageDialog(null, "No log to save!");
	                return;
	            }
	            
	            
	            //ask user for save location
	            int returnVal = fc.showSaveDialog(null);

	            if (returnVal == JFileChooser.APPROVE_OPTION) {
	                File file = fc.getSelectedFile();
	                saveTextLog(file.getAbsolutePath(), lblLog.getText());
	            }
	            
			}
		});
		btnSaveLog.setBounds(66, 163, 110, 23);
		contentPane.add(btnSaveLog);
		
		JButton btnClearLog = new JButton("Clear Log");
		btnClearLog.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				//clear the log
	            if (JOptionPane.YES_OPTION == JOptionPane.showConfirmDialog(null, "Clear the log?"))
	            {
	                //clear log
	                lblLog.setText("");
	            }
			}
		});
		btnClearLog.setBounds(206, 163, 109, 23);
		contentPane.add(btnClearLog);
		
		btnConnect = new JButton("Connect");
		btnConnect.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent arg0) {
				if (btnConnect.getText().equals("Connect"))
	            {

	                int port = -1;    
	                try{
		                port = Integer.parseInt(txtPort.getText());
	                } catch (NumberFormatException e){
	                	port = DEFAULT_PORT;
	                }

	                WriteLogLine("Attempting to connect to: " + txtUsername.getText() + "@" + txtURL.getText() + ":" + port);

	                
	                //setup the client's callbacks, then connect
	                client = new Client(txtUsername.getText(), txtURL.getText(), port);
	                client.addConnectListener(new EventListener() {
	        			@Override
	        			public void call(Object sender, final Object... args) {
	        				SwingUtilities.invokeLater(new Runnable() {
								@Override
								public void run() {
									enableConnectButton(false);
			        				WriteLogLine("Connection Successful");									
								}
							});
	        			}
	        		});
	                client.addDisconnectListener(new EventListener() {
	        			@Override
	        			public void call(Object sender, final Object... args) {
	        				SwingUtilities.invokeLater(new Runnable() {
								@Override
								public void run() {
									enableConnectButton(true);
			        				WriteLogLine("Client Disconnected");									
								}
							});
	        			}
	        		});
	                client.addMessageListener(new EventListener() {
	        			@Override
	        			public void call(Object sender, final Object... args) {
	        				SwingUtilities.invokeLater(new Runnable() {
								@Override
								public void run() {
									Message m = (Message) args[0];
						            WriteLogLine(m.toString());
								}
							});
	        			}
	        		});
	                client.addErrorListener(new EventListener() {
	        			@Override
	        			public void call(Object sender, final Object... args) {
	        				SwingUtilities.invokeLater(new Runnable() {
								@Override
								public void run() {
									Exception e = (Exception) args[0];
			        				WriteLogLine(e.getMessage() + "\n" + e.getStackTrace());									
								}
							});
	        			}
	        		});
	                client.startConnection();

	                enableConnectButton(true);
	            }
	            else if (btnConnect.getText().equals("Disconnect"))
	            {
	                enableConnectButton(false);
	                client.closeConnection();
	            }
			}
		});
		btnConnect.setBounds(466, 113, 116, 23);
		contentPane.add(btnConnect);
		
		txtURL = new JTextField();
		txtURL.setText("http://127.0.0.1");
		txtURL.setBounds(402, 11, 180, 20);
		contentPane.add(txtURL);
		txtURL.setColumns(10);
		
		txtPort = new JTextField();
		txtPort.setText("8000");
		txtPort.setBounds(402, 45, 180, 20);
		contentPane.add(txtPort);
		txtPort.setColumns(10);
		
		txtUsername = new JTextField();
		txtUsername.setText("demo_test_username");
		txtUsername.setColumns(10);
		txtUsername.setBounds(402, 82, 180, 20);
		contentPane.add(txtUsername);
		
		pnlStatus = new JPanel();
		pnlStatus.setToolTipText("Disconnected");
		pnlStatus.setBackground(Color.RED);
		pnlStatus.setBounds(402, 114, 22, 22);
		contentPane.add(pnlStatus);
		
		chkDeliverToSelf = new JCheckBox("Deliver to Self?");
		chkDeliverToSelf.setBounds(172, 33, 122, 31);
		contentPane.add(chkDeliverToSelf);
		
		JLabel lblLogHeader = new JLabel("Log:");
		lblLogHeader.setFont(new Font("Tahoma", Font.BOLD, 11));
		lblLogHeader.setBounds(10, 167, 46, 14);
		contentPane.add(lblLogHeader);
		
		lblLog = new JTextPane();
		
		JScrollPane scrollPane = new JScrollPane(lblLog);
		scrollPane.setHorizontalScrollBarPolicy(ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
		scrollPane.setVerticalScrollBarPolicy(ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS);
		scrollPane.setBounds(10, 197, 572, 261);
		contentPane.add(scrollPane);
		
        enableConnectButton(true);
	}
	
	
    //changes interface for when connected or not
    private void enableConnectButton(boolean enabled)
    {
        if (enabled == true)
        {
            btnConnect.setText("Connect");
            btnSend.setToolTipText("Please connect to a server first");
            btnSend.setEnabled(false);
            pnlStatus.setBackground(Color.red);
            pnlStatus.setToolTipText("Disconnected");
        }
        else
        {  
            btnConnect.setText("Disconnect");
            btnSend.setToolTipText("");
            btnSend.setEnabled(true);
            pnlStatus.setBackground(new Color(34, 139, 34));
            pnlStatus.setToolTipText("Connected to: " + client.getUsername() + "@" + client.getURL() + ":" + client.getPort() );
            
        }
    }
	
	
	
	
	
	private void WriteLogLine(String message)
	{
		 
	    String addText = lblLog.getText() + "[" + timeStamp() + "] " + message + "\n";
	    lblLog.setText(addText);
	    //scrollLogToBottom();
	
	}
	
	private static DateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");

	private String timeStamp()
	{
		Date now = new Date(System.currentTimeMillis());
		return dateFormat.format(now);
	}
	
	private void saveTextLog(String path, String text){
		try {
			PrintWriter out = new PrintWriter(path);
			out.write(text);
			out.flush();
			out.close();
		} catch (IOException  e) {
			JOptionPane.showMessageDialog(null, "Error! Could not save log" + e.getMessage());
			e.printStackTrace();
		}
		
	}
}
