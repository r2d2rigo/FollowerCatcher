@echo off
setlocal
set PATH="C:\Program Files (x86)\Windows Kits\8.0\bin\x86";%PATH%
fxc.exe /T vs_4_0_level_9_1 DefaultShader.fx /E VS /Fo DefaultShaderVS.fxo
fxc.exe /T ps_4_0_level_9_1 DefaultShader.fx /E PS /Fo DefaultShaderPS.fxo
endlocal