package com.example.android.networkitandroid;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.TextView;

import networkit.Client;

public class MainActivity extends AppCompatActivity {

    private Button btnSendPoke;
    private TextView lblCount;
    private TextView lblConnectionStatus;
    private CheckBox chkDeliverToSelf;

    private Client connection;

    private int count = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        btnSendPoke = (Button) findViewById(R.id.btnSend);
        lblCount = (TextView) findViewById(R.id.lblCount);
        lblConnectionStatus = (TextView) findViewById(R.id.lblConnectStatus);
        chkDeliverToSelf = (CheckBox) findViewById(R.id.chkDeliverToSelf);

        btnSendPoke.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                if (connection == null){
                    return;
                }

                networkit.Message m = new networkit.Message("Poke!");
                m.addField("num1", Integer.toString(1));
                m.addField("num1", Integer.toString(9));
                m.addField("count", Integer.toString(count++));

                m.deliverToSelf = chkDeliverToSelf.isChecked();

                connection.sendMessage(m);
            }
        });

        connection = new Client("demo_test_username", "162.246.156.110", 8000);
        connection.addConnectListener(new Client.EventListener() {
            @Override
            public void call(Object o, final Object... objects) {
                MainActivity.super.runOnUiThread(new Runnable() {               //needs to run on GUI thread
                    @Override
                    public void run() {
                        lblConnectionStatus.setText("Connected!");
                    }
                });
            }
        });

        connection.addMessageListener(new Client.EventListener() {
            @Override
            public void call(Object o, final Object... objects) {
                MainActivity.super.runOnUiThread(new Runnable() {               //needs to run on GUI thread
                    @Override
                    public void run() {
                        networkit.Message m = (networkit.Message) objects[0];
                        lblCount.setText("Count="+ Integer.parseInt(m.getField("count")));
                    }
                });
            }
        });

        connection.addDisconnectListener(new Client.EventListener() {
            @Override
            public void call(Object o, final Object... objects) {
                MainActivity.super.runOnUiThread(new Runnable() {               //needs to run on GUI thread
                    @Override
                    public void run() {
                        lblConnectionStatus.setText("Disconnected");
                    }
                });
            }
        });

        connection.addErrorListener(new Client.EventListener() {
            @Override
            public void call(Object o, final Object... objects) {
                MainActivity.super.runOnUiThread(new Runnable() {               //needs to run on GUI thread
                    @Override
                    public void run() {
                        lblConnectionStatus.setText("Error!" + objects[0].toString());
                    }
                });
            }
        });


        connection.startConnection();
    }
}
