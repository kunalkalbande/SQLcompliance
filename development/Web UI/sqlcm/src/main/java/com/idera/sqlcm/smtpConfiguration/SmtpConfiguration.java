package com.idera.sqlcm.smtpConfiguration;

import java.util.Properties;

import javax.mail.Authenticator;
import javax.mail.PasswordAuthentication;
import javax.mail.Session;

import org.apache.commons.mail.Email;
 
public class SmtpConfiguration{
 
    /**
       Outgoing Mail (SMTP) Server
       requires TLS or SSL: smtp.gmail.com (use authentication)
       Use Authentication: Yes
       Port for SSL: 465
     */
   
	final String fromEmail = "abhay.singh@xorlabs.com"; //requires valid gmail id
    final String password = "abhay@26"; // correct password for gmail id
    final String toEmail = "abhay.singh@xorlabs.com"; // can be any email id 
         
    public void SendSMTPMail(){
        Properties props = new Properties();
        props.put("mail.smtp.host", "smtp.gmail.com"); //SMTP Host
        props.put("mail.smtp.socketFactory.port", "465"); //SSL Port
        props.put("mail.smtp.socketFactory.class","javax.net.ssl.SSLSocketFactory"); //SSL Factory Class
        props.put("mail.smtp.auth", "true"); //Enabling SMTP Authentication
        props.put("mail.smtp.port", "465"); //SMTP Port
         
        Authenticator auth = new Authenticator() {
            //override the getPasswordAuthentication method
            protected PasswordAuthentication getPasswordAuthentication() {
                return new PasswordAuthentication(fromEmail, password);
            }
        };
         
        Session session = Session.getDefaultInstance(props, auth);
            EmailUtil.sendEmail(session, toEmail,"SSLEmail Testing Subject", "SSLEmail Testing Body");
 
          }
    }