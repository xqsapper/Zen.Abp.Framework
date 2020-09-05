echo "zen-arch:welcome to use ZenWinSrv"
echo "stop windows service..."
..\ZenWinSrv.exe stop
echo "stop windows service success"
echo "uninstall windows service..."
..\ZenWinSrv.exe uninstall
echo "uninstall windows service success"
pause