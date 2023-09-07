# simap-soapgrabber
This is a little #C application that uses the simap Webserver / API to get data from the swiss public procurement platform [SIMAP](https://simap.ch/shabforms/COMMON/application/applicationGrid.jsp?template=1&view=1&page=/MULTILANGUAGE/simap/content/start.jsp&language=EN{:target="_blank"})

# Soap Setup Guide
For others of my generation who have the honor of working with soap and who have to generate classes through wsdl, this is how I managed to make it work:

Step 1:
Use .NET (Java doesn't support wsdl anymore. To use this old feature you'd have to use and old sdk.) 

Step 2:
Create a new Windows Forms App (with the .Net Framework)

Step 3:
Add a Reference, input your wsdl link

Step 4: 
Go to Advanced settings, Add Web Reference, input your wsdl link

The end result should look something like this: 

![image](https://github.com/DeHess/simap-soapgrabber/assets/56408082/1d076fee-421d-4d8d-94d7-3bddb0acd4da)

Now you can create your client in your code and make the first calls

