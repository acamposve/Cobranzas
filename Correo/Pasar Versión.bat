xcopy %1*.exe "\\VECCSVS014\Cobranzas\Servicios\Correo\PasarVersion" /Y
xcopy %1*.dll "\\VECCSVS014\Cobranzas\Servicios\Correo\PasarVersion" /Y
xcopy %1en "\\VECCSVS014\Cobranzas\Servicios\Correo\PasarVersion\en\" /S /Y
xcopy %1pt "\\VECCSVS014\Cobranzas\Servicios\Correo\PasarVersion\pt\" /S /Y
echo %1
pause