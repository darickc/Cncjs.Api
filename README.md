# Cncjs.Api
### A CNCjs pendant geared toward midsized screens like the 7" raspberry pi touch screen.

Makes the ui for CNCjs more touch friendly while still allowing for much of the functionality.  

### Setup
I recomend building the code on a computer other than the raspberry pi and copying the code to the pi.  See https://docs.microsoft.com/en-us/dotnet/iot/deployment for more general isntructions.

1. Download and install the .Net sdk for your platform (https://dotnet.microsoft.com/en-us/download).
2. Clone the repository
``` 
git clone https://github.com/darickc/Cncjs.Api.git
```
3. Open up src/CncJs.Pendant.Web and build the code for your platform. For example, for the raspberry pi use the below. If using a platform other than a rasberry pi, see https://docs.microsoft.com/en-us/dotnet/core/rid-catalog for the different runtime identifiers.
```
dotnet publish -c Release -r linux-arm --self-contained
```
4. Go to src/CncJs.Penant.Web/bin/Release/net6.0/linux-arm/publish (or the runtime identier you used) and copy to the raspberry pi (or whatever computer you are using).  This command will copy it to the folder webpenant located on the pi.  I believe the folder needs to be created on the pi first.
```
scp -r * pi@raspberrypi:/home/pi/webpendant/
```
5. Login to the Raspberry pi and go to the webpendant (or whatever folder you used above).  Then create a settings.json file and make the executable execute permission.
```
cd webpendant
chmod +x CncJs.Pendant.Web
nano settings.json
```
6. On the pi

Add secret for token:
```
{
  "Secret": "",
  "SocketPort": 53346
}
```


To be continued
