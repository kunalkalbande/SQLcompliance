package com.idera;

import java.io.DataInputStream;
import java.io.IOException;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;

public class Monitor extends Thread {

    protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(Monitor.class);
    protected static final org.apache.log4j.Logger debug = org.apache.log4j.Logger.getLogger(Monitor.class);

    public static final int DEFAULT_STOP_PORT = 8064;

    public static final String STOP_COMMAND = "stop-buserver-981gqsdy809123eiqewd294d";
    public static final String STATUS_COMMAND = "status-buserver-981gqsdy809123eiqewd294d";
    public static final String RESTART_COMMAND = "restart-buserver-981gqsdy809123eiqewd294d";

    private ServerSocket serverSock;
    // private MonitorProtocol server;

    /**
     *
     */
    public Monitor(MonitorProtocol server_, int stopPort) throws Exception {
        int port = Integer.getInteger("buserver.stop.port", DEFAULT_STOP_PORT).intValue();
        port = stopPort;
        // server = server_;
        // FIXME check for server_
        try {
            if (port < 0) {
                throw new Exception("buserver.stop.port (" + port + ") invalid.");
            }
            this.serverSock = new ServerSocket(port, 10, InetAddress.getByName("0.0.0.0"));
            if (port == 0) {
                port = this.serverSock.getLocalPort();
            }
        } catch (IOException ioe) {
            throw new Exception("failed to start stop monitor.", ioe);
        }
        if (this.serverSock != null) {
        } else {
            throw new Exception("failed to start stop monitor.");
        }
        if (debug.isDebugEnabled()) {
            debug.debug("stop monitor listening on 127.0.0.1:" + port);
        }
        // comment this when main thread does work
        // run();
    }

    @Override
    public void run() {
        boolean isContinue = true;
        while (isContinue) {
            Socket socket = null;
            try {
                socket = this.serverSock.accept();
                logger.info("New connection established");
                DataInputStream dis = new DataInputStream(socket.getInputStream());
                byte data[] = new byte[1000];
                dis.read(data);
                String cmd = new String(data).trim();
                if (debug.isDebugEnabled()) {
                    debug.debug("read (" + cmd + ").");
                }
                if (STOP_COMMAND.equals(cmd)) {
                    try {
                        logger.info("Stopping the monitor socket");
                        socket.close();
                        this.serverSock.close();
                    } catch (IOException ioe) {
                        debug.error("error closing stop monitor socket.");
                    } finally {
                        isContinue = false;
                    }
                } else if (STATUS_COMMAND.equals(cmd)) {
                    logger.info("recieved signal for stoping tomcat");
                    socket.getOutputStream().write("OK\r\n".getBytes());
                    socket.getOutputStream().flush();
                } else if (RESTART_COMMAND.equals(cmd)) {
                    logger.info("recieved signal for restarting tomcat");
                    boolean restart = Main.restart();
                    String message = "tomcat restarted: " + restart;
                    socket.getOutputStream().write(cmd.getBytes());
                    socket.getOutputStream().flush();
                }
            } catch (IOException ioe) {
                debug.error("stop monitor IO error.", ioe);
            } finally {
                if (socket != null) {
                    try {
                        logger.info("Retrying to stop the monitor socket");
                        socket.close();
                    } catch (Exception e) {
                        // ignore exception
                        debug.error("stop monitor exception.", e);
                    }
                }
                socket = null;
            }
        }
    }

    /**
     * Start a Monitor. This static method starts a monitor that listens for
     * admin requests.
     */
    public static Monitor monitor(MonitorProtocol server_, int stopPort) throws Exception {
        return new Monitor(server_, stopPort);
    }
}
