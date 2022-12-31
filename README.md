# WakeOnLAN
A set of Windows applications to send Wake On LAN packets to turn on the office PC remotely.

### Sample usage
Within the office network:
1. Set up a machine to run the **WakeOnLANServer.exe** (can set it to start automatically using *Task Scheduler*).
2. Allow Ping request through firewall on server machine.
3. Set up Wake On LAN on your office PC (typically some BIOS and Windows settings).
4. Allow Ping request through firewall on your office PC.
5. Enable Remote Desktop on your office PC.

Within the remote network:
1. Connect to office VPN (so that packets can be sent to the server machine from your remote machine).
2. Launch **WakeOnLANClient.exe** (and allow it through firewall).
![WakeOnLANClient.exe UI](https://raw.githubusercontent.com/simon-yeunglm/WakeOnLAN/main/screenshots/client_ui.png)
3. Enter the **Server IP** and **MAC Address** of your office PC.
4. Click the **Wake** button.
5. *Windows Remote Desktop App* will be launched automatically if your office PC is turned on.
![WakeOnLANClient.exe connect success](https://raw.githubusercontent.com/simon-yeunglm/WakeOnLAN/main/screenshots/client_connect.png)
